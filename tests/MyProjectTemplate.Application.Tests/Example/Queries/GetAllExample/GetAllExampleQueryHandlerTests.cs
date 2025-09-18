using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Application.Example.Queries.GetAllExample;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Domain.Interfaces;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Queries.GetAllExample;

public class GetAllExampleQueryHandlerTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly Mock<ILogger<GetAllExampleQueryHandler>> _loggerMock = new();
    private readonly GetAllExampleQueryHandler _sut;
    private readonly Fixture _fixture;

    public GetAllExampleQueryHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new GetAllExampleQueryHandler(_repoMock.Object, _loggerMock.Object);   
    }

    [Fact]
    public async Task Handle_ShouldReturnDtos_WhenRepositoryReturnsEntities()
    {
        // Arrange
        var entities = new[]
        {
            new Domain.Entities.Example("A", "DA", new DateTime(2025,1,1), "LA", Difficulty.Easy, 1, 2),
            _fixture.Create<Domain.Entities.Example>()
        };
        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(entities);
        
        // Act
        var result = await _sut.Handle(new GetAllExampleQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().BeEmpty();

        var list = result.Data.Should().NotBeNull().And.BeAssignableTo<IEnumerable<ExampleDto>>().Subject.ToList();
        list.Should().HaveCount(2);
        list[0].Name.Should().Be("A");
        list[0].Description.Should().Be("DA");
        list[0].Location.Should().Be("LA");
        list[0].Difficulty.Should().Be(Difficulty.Easy);
        list[0].Latitude.Should().Be(1);
        list[0].Longitude.Should().Be(2);

        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(Array.Empty<Domain.Entities.Example>());

        // Act
        var result = await _sut.Handle(new GetAllExampleQuery(), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_Canceled_ShouldReturnFailureWithEmptyData()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _sut.Handle(new GetAllExampleQuery(), cts.Token);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Operation canceled");
        result.Data.Should().NotBeNull().And.BeEmpty();

        _repoMock.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldBubbleException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync())
                 .ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        var act = async () => await _sut.Handle(new GetAllExampleQuery(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("boom");
    }
}
