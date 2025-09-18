using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.Commands.CreateExample;
using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Domain.Interfaces;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Commands.CreateExample;

public class CreateExampleCommandHandlerTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly Mock<ILogger<CreateExampleCommandHandler>> _loggerMock = new();
    private readonly CreateExampleCommandHandler _sut;

    public CreateExampleCommandHandlerTests()
    {
        _sut = new CreateExampleCommandHandler(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreate_AndReturnSuccessResult()
    {
        // Arrange
        var cmd = new CreateExampleCommand
        {
            Name = "Event",
            Description = "Desc",
            Date = new DateTime(2025, 1, 1),
            Location = "Loc",
            Difficulty = Difficulty.Medium,
            Latitude = 1.23m,
            Longitude = 4.56m
        };

        var saved = new Domain.Entities.Example(cmd.Name, cmd.Description, cmd.Date, cmd.Location, cmd.Difficulty, cmd.Latitude, cmd.Longitude);

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Example>()))
                 .ReturnsAsync(saved);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Created successfully");
        result.Should().BeOfType<BaseResult<CreateExampleResponse>>();
        var data = (result as BaseResult<CreateExampleResponse>)!.Data;
        data.Should().NotBeNull();
        data.Id.Should().Be(saved.Id);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Example>()), Times.Once);
        _loggerMock.Verify(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, __) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldPassMappedEntityToRepository()
    {
        // Arrange
        Domain.Entities.Example? captured = null;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Example>()))
                 .Callback<Domain.Entities.Example>(e => captured = e)
                 .ReturnsAsync(() => captured!);

        var cmd = new CreateExampleCommand
        {
            Name = "N",
            Description = "D",
            Date = new DateTime(2025, 2, 2),
            Location = "L",
            Difficulty = Difficulty.Hard,
            Latitude = 10.1m,
            Longitude = 20.2m
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Name.Should().Be(cmd.Name);
        captured.Description.Should().Be(cmd.Description);
        captured.Date.Should().Be(cmd.Date);
        captured.Location.Should().Be(cmd.Location);
        captured.Difficulty.Should().Be(cmd.Difficulty);
        captured.Latitude.Should().Be(cmd.Latitude);
        captured.Longitude.Should().Be(cmd.Longitude);

        result.Data.Id.Should().Be(captured.Id);
    }

    [Fact]
    public async Task Handle_ShouldRespectCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var cmd = new CreateExampleCommand { Name = "X" };

        // Act
        var act = async () => await _sut.Handle(cmd, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Example>()), Times.Never);
    }
}
