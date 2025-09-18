using Dodo.Primitives;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.Commands.DeleteExample;
using MyProjectTemplate.Core.Exceptions;
using MyProjectTemplate.Domain.Interfaces;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Commands.DeleteExample;

public class DeleteExampleCommandHandlerTests
{
    private readonly Mock<IExampleRepository> _repoMock = new();
    private readonly Mock<ILogger<DeleteExampleCommandHandler>> _loggerMock = new();
    private readonly DeleteExampleCommandHandler _sut;

    public DeleteExampleCommandHandlerTests()
    {
        _sut = new DeleteExampleCommandHandler(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidIdFormat_ShouldReturnFailureBaseResult()
    {
        // Arrange
        var cmd = new DeleteExampleCommand("not-a-uuid");

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("The example ID is invalid.");
        _repoMock.Verify(r => r.ExistsAsync(It.IsAny<Uuid>()), Times.Never);
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Uuid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Uuid.CreateVersion7().ToString();
        _repoMock.Setup(r => r.ExistsAsync(It.IsAny<Uuid>())).ReturnsAsync(false);

        var cmd = new DeleteExampleCommand(id);

        // Act
        var act = async () => await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage($"Example not found. Id={id}");
        _repoMock.Verify(r => r.ExistsAsync(It.IsAny<Uuid>()), Times.Once);
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Uuid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidId_ExistingEntity_ShouldDelete_AndReturnSuccess()
    {
        // Arrange
        var uuid = Uuid.CreateVersion7();
        var cmd = new DeleteExampleCommand(uuid.ToString());

        _repoMock.Setup(r => r.ExistsAsync(uuid)).ReturnsAsync(true);
        _repoMock.Setup(r => r.DeleteAsync(uuid)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be($"Successfully deleted Example Id={cmd.Id}");
        _repoMock.Verify(r => r.ExistsAsync(uuid), Times.Once);
        _repoMock.Verify(r => r.DeleteAsync(uuid), Times.Once);
    }

    [Fact]
    public async Task Handle_Cancellation_ShouldReturnCanceledBaseResult_AndNotCallRepo()
    {
        // Arrange
        var sut = new DeleteExampleCommandHandler(_repoMock.Object, _loggerMock.Object);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var cmd = new DeleteExampleCommand(Uuid.CreateVersion7().ToString());

        // Act
        var result = await _sut.Handle(cmd, cts.Token);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Operation canceled");
        _repoMock.Verify(r => r.ExistsAsync(It.IsAny<Uuid>()), Times.Never);
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Uuid>()), Times.Never);
    }
}
