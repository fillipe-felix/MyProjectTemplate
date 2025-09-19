using FluentAssertions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Api.Controllers;
using MyProjectTemplate.Application.Example.Commands.CreateExample;
using MyProjectTemplate.Application.Example.Commands.DeleteExample;
using MyProjectTemplate.Application.Example.Commands.UpdateExample;
using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Application.Example.Queries.GetAllExample;
using MyProjectTemplate.Application.Example.Queries.GetByIdExample;
using MyProjectTemplate.Core.Base;

using Xunit;

namespace MyProjectTemplate.Api.Tests.Controllers;

public class ExampleControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<ExampleController>> _loggerMock = new();
    private readonly ExampleController _sut;

    public ExampleControllerTests()
    {
        _sut = new ExampleController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOk_WithResult()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        var dto = new ExampleDto { Name = "A" };
        var result = new BaseResult<ExampleDto>(dto);
        _mediatorMock.Setup(m => m.Send(It.Is<GetByIdExampleQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.GetByIdAsync(id, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)response.Result!;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().Be(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenResultIsNull_ShouldReturnNotFound()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetByIdExampleQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((BaseResult<ExampleDto>?)null);

        // Act
        var response = await _sut.GetByIdAsync(id, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOk_WithResult()
    {
        // Arrange
        var pagination = new PaginationParams(1, 10);
        var data = new PagedResult<ExampleDto>
        {
            Data = new List<ExampleDto> { new() { Name = "A" } },
            TotalCount = 1,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        _mediatorMock.Setup(m => m.Send(It.Is<GetAllExampleQuery>(q => q.Pagination.PageNumber == pagination.PageNumber &&
                                                                       q.Pagination.PageSize == pagination.PageSize),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        // Act
        var response = await _sut.GetAllAsync(pagination,CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)response.Result!;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().Be(data);
    }

    [Fact]
    public async Task CreateAsync_NullCommand_ShouldReturnBadRequest()
    {
        // Act
        var response = await _sut.CreateAsync(null!, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedAt_WithResponse()
    {
        // Arrange
        var cmd = new CreateExampleCommand { Name = "N" };
        var id = Dodo.Primitives.Uuid.CreateVersion7();
        var result = new BaseResult<CreateExampleResponse>(new CreateExampleResponse(id), true, "ok");
        _mediatorMock.Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.CreateAsync(cmd, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<CreatedAtRouteResult>();
        var created = (CreatedAtRouteResult)response.Result!;
        created.RouteName.Should().Be(nameof(ExampleController.GetByIdAsync));
        created.Value.Should().Be(result);
        created.RouteValues!["id"].Should().Be(id);
        created.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task UpdateAsync_NullCommand_ShouldReturnBadRequest()
    {
        // Act
        var response = await _sut.UpdateAsync(Guid.NewGuid().ToString(), null!, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ShouldReturnBadRequest_WithBaseResult()
    {
        // Arrange
        var cmd = new UpdateExampleCommand();

        // Act
        var response = await _sut.UpdateAsync("not-a-uuid", cmd, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var bad = (BadRequestObjectResult)response;
        bad.Value.Should().BeOfType<BaseResult>();
        ((BaseResult)bad.Value!).Success.Should().BeFalse();
        ((BaseResult)bad.Value!).Message.Should().Be("The example ID is invalid.");
    }

    [Fact]
    public async Task UpdateAsync_WhenHandlerReturnsFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        var cmd = new UpdateExampleCommand();
        var result = new BaseResult(false, "err");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateExampleCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.UpdateAsync(id, cmd, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var bad = (BadRequestObjectResult)response;
        bad.Value.Should().Be(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenSuccess_ShouldReturnNoContent()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        var cmd = new UpdateExampleCommand();
        var result = new BaseResult(true, "ok");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateExampleCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.UpdateAsync(id, cmd, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenHandlerReturnsFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        var result = new BaseResult(false, "err");
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteExampleCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.DeleteAsync(id, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var bad = (BadRequestObjectResult)response;
        bad.Value.Should().Be(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenSuccess_ShouldReturnNoContent()
    {
        // Arrange
        var id = Dodo.Primitives.Uuid.CreateVersion7().ToString();
        var result = new BaseResult(true, "ok");
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteExampleCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(result);

        // Act
        var response = await _sut.DeleteAsync(id, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}
