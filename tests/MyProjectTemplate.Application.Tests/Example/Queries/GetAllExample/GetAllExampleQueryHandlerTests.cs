using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using MyProjectTemplate.Application.Example.DTOs;
using MyProjectTemplate.Application.Example.Queries.GetAllExample;
using MyProjectTemplate.Core.Base;
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
    public async Task Handle_ShouldReturnPagedDtos_WhenRepositoryReturnsPagedEntities()
    {
        // Arrange
        var pagination = new PaginationParams(pageNumber: 2, pageSize: 1);
        var entities = new List<Domain.Entities.Example>
        {
            new ("A", "DA", new DateTime(2025,1,1), "LA", Difficulty.Easy, 1, 2),
            new ("B", "DB", new DateTime(2025,2,2), "LB", Difficulty.Medium)
        };

        var pageItems = entities.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
        var totalCount = entities.Count;

        _repoMock
            .Setup(r => r.GetAllAsync(
                It.Is<PaginationParams>(p => p.PageNumber == pagination.PageNumber && p.PageSize == pagination.PageSize),
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Example, bool>>>(),
                It.IsAny<Func<IQueryable<Domain.Entities.Example>, IOrderedQueryable<Domain.Entities.Example>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((pageItems, totalCount));

        // Act
        var result = await _sut.Handle(new GetAllExampleQuery(pagination), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        var data = result.Should().NotBeNull().And.BeAssignableTo<PagedResult<ExampleDto>>().Subject;

        data.TotalCount.Should().Be(totalCount);
        data.PageNumber.Should().Be(pagination.PageNumber);
        data.PageSize.Should().Be(pagination.PageSize);
        data.TotalPages.Should().Be((int)Math.Ceiling(totalCount / (double)pagination.PageSize));
        data.Data.Should().HaveCount(pageItems.Count);
        data.Data.First().Name.Should().Be("B");

        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldBubbleException()
    {
        // Arrange
        var pagination = new PaginationParams(1, 10);
        _repoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<PaginationParams>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Example, bool>>>(),
                It.IsAny<Func<IQueryable<Domain.Entities.Example>, IOrderedQueryable<Domain.Entities.Example>>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        var act = async () => await _sut.Handle(new GetAllExampleQuery(pagination), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
    }
}
