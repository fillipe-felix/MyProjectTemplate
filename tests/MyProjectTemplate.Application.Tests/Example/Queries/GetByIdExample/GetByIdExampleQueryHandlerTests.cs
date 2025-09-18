using AutoFixture;

using Dodo.Primitives;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Application.Example.Queries.GetByIdExample;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Domain.Interfaces;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Queries.GetByIdExample;

public class GetByIdExampleQueryHandlerTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly Mock<ILogger<GetByIdExampleQueryHandler>> _loggerMock = new();
    private readonly GetByIdExampleQueryHandler _sut;
    private readonly Fixture _fixture;

    public GetByIdExampleQueryHandlerTests()
    {
        _fixture = new Fixture();
        _sut = new GetByIdExampleQueryHandler(_repoMock.Object, _loggerMock.Object);   
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenEntityExists()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        var entity = _fixture.Create<Domain.Entities.Example>();

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        var result = await _sut.Handle(new GetByIdExampleQuery(id.ToString()), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().BeEmpty();

        var dto = result.Data.Should().NotBeNull().And.BeOfType<ExampleDto>().Subject;
        dto.Name.Should().Be(entity.Name);
        dto.Description.Should().Be(entity.Description);
        dto.Location.Should().Be(entity.Location);
        dto.Difficulty.Should().Be(entity.Difficulty);

        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenEntityIsNull()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Domain.Entities.Example?)null);

        var query = new GetByIdExampleQuery(id.ToString());

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"Example not found. Id={query.Id}");
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_Canceled_ShouldReturnFailureResult()
    {
        // Arrange
        var id = Uuid.CreateVersion7().ToString();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _sut.Handle(new GetByIdExampleQuery(id), cts.Token);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Operation canceled");
        result.Data.Should().BeNull();
        _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Uuid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepoThrows_ShouldBubbleException()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        _repoMock.Setup(r => r.GetByIdAsync(id))
                 .ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        var act = async () => await _sut.Handle(new GetByIdExampleQuery(id.ToString()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("boom");
    }
}
