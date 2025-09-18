using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.Commands.UpdateExample;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Domain.Interfaces;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Commands.UpdateExample;

public class UpdateExampleCommandHandlerTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly Mock<ILogger<UpdateExampleCommandHandler>> _loggerMock = new();
    private readonly UpdateExampleCommandHandler _sut;

    public UpdateExampleCommandHandlerTests()
    {
        _sut = new UpdateExampleCommandHandler(_repoMock.Object, _loggerMock.Object);
    }

    private static UpdateExampleCommand BuildCommand() => new()
    {
        Id = Dodo.Primitives.Uuid.CreateVersion7(),
        Name = "Updated Name",
        Description = "Updated Description",
        Date = new DateTime(2026, 1, 2, 10, 0, 0, DateTimeKind.Utc),
        Location = "Updated Location",
        Latitude = 12.34m,
        Longitude = 56.78m,
        Difficulty = Difficulty.Hard
    };

    [Fact]
    public async Task Handle_ShouldUpdateEntity_AndReturnSuccess()
    {
        // Arrange
        var existing = new Domain.Entities.Example("N", "D", new DateTime(2025, 1, 1), "L", Difficulty.Easy, 0m, 0m);
        var cmd = BuildCommand();

        _repoMock.Setup(r => r.GetByIdAsync(cmd.Id))
                 .ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(existing))
                 .Returns(Task.CompletedTask);
        
        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be($"Successfully updated Example Id={existing.Id}");

        existing.Name.Should().Be(cmd.Name);
        existing.Description.Should().Be(cmd.Description);
        existing.Date.Should().Be(cmd.Date);
        existing.Location.Should().Be(cmd.Location);
        existing.Latitude.Should().Be(cmd.Latitude);
        existing.Longitude.Should().Be(cmd.Longitude);
        existing.Difficulty.Should().Be(cmd.Difficulty);

        _repoMock.Verify(r => r.GetByIdAsync(cmd.Id), Times.Once);
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRespectCancellationToken_AndReturnCanceledResult()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var cmd = BuildCommand();

        // Act
        var result = await _sut.Handle(cmd, cts.Token);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Operation canceled");

        _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Dodo.Primitives.Uuid>()), Times.Never);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Example>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldBubbleException()
    {
        // Arrange
        var cmd = BuildCommand();
        _repoMock.Setup(r => r.GetByIdAsync(cmd.Id))
                 .ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        var act = async () => await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("boom");
    }
}
