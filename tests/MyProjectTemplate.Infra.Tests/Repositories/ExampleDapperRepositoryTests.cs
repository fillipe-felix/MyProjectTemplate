using System.Data;

using AutoFixture;

using Dapper;

using Dodo.Primitives;

using FluentAssertions;

using Moq;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Entities;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Infra.Contracts;
using MyProjectTemplate.Infra.Repositories;

using Xunit;

namespace MyProjectTemplate.Infra.Tests.Repositories;

public class ExampleDapperRepositoryTests
{
    private readonly Mock<IDapperWrapper> _dapperMock;
    private readonly ExampleDapperRepository _repo;
    private readonly Fixture _fixture;

    public ExampleDapperRepositoryTests()
    {
        _dapperMock = new Mock<IDapperWrapper>(MockBehavior.Strict);
        _repo = new ExampleDapperRepository(_dapperMock.Object);
        _fixture = new Fixture();  
    }

    [Fact]
    public async Task GetByIdAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        var expected = _fixture.Create<Example>();
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"
                            SELECT 
                                Id,
                                Name,
                                Description,
                                Date,
                                Location,
                                Latitude,
                                Longitude,
                                Difficulty,
                                Active,
                                CreatedAt,
                            FROM Examples
                            WHERE Id = @Id";

        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((sql, dp) =>
            {
                capturedQuery = sql;
                capturedParams = dp;
            })
            .ReturnsAsync(new List<Example> { expected });

        // Act
        await _repo.GetByIdAsync(id);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.AddDynamicParams(new { Id = id });
        capturedParams.ParameterNames.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnFirstItem_WhenExists()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        var expected = _fixture.Create<Example>();

        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(new List<Example> { expected });

        // Act
        var result = await _repo.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(expected.Name);

        _dapperMock.Verify(d => d.QueryAsync<Example>(It.Is<string>(s => s.Contains("SELECT")), It.IsAny<DynamicParameters>()), Times.Once);
        _dapperMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(Enumerable.Empty<Example>());

        // Act
        var result = await _repo.GetByIdAsync(Uuid.CreateVersion7());

        // Assert
        result.Should().BeNull();
        _dapperMock.VerifyAll();
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var pagination = new PaginationParams(2, 3);
        var items = new List<Example>
        {
            new("A","D", System.DateTime.UtcNow, "L", Difficulty.Easy),
            new("B","D", System.DateTime.UtcNow, "L", Difficulty.Medium),
            new("C","D", System.DateTime.UtcNow, "L", Difficulty.Hard)
        };
        var total = new List<int> { 10 };
        
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"
                            SELECT 
                                Id,
                                Name,
                                Description,
                                Date,
                                Location,
                                Latitude,
                                Longitude,
                                Difficulty,
                                Active,
                                CreatedAt
                            FROM Examples
                            ORDER BY CreatedAt DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.Is<string>(s => s.Contains("ORDER BY CreatedAt DESC")), It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters?>((sql, dynamicParameters) =>
            {
                capturedQuery = sql;
                capturedParams = dynamicParameters;
            })
            .ReturnsAsync(items);
        
        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.Is<string>(s => s.Contains("COUNT(1) FROM Examples")), null))
            .ReturnsAsync(total);

        // Act
        await _repo.GetAllAsync(pagination);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.ParameterNames.Should().Contain("Offset");
        capturedParams.ParameterNames.Should().Contain("PageSize");
        capturedParams.ParameterNames.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedItemsAndTotal()
    {
        // Arrange
        var pagination = new PaginationParams(2, 3);
        var items = new List<Example>
        {
            new("A","D", System.DateTime.UtcNow, "L", Difficulty.Easy),
            new("B","D", System.DateTime.UtcNow, "L", Difficulty.Medium),
            new("C","D", System.DateTime.UtcNow, "L", Difficulty.Hard)
        };
        var total = new List<int> { 10 };

        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.Is<string>(s => s.Contains("ORDER BY CreatedAt DESC")), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(items);

        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.Is<string>(s => s.Contains("COUNT(1) FROM Examples")), null))
            .ReturnsAsync(total);

        // Act
        var (resultItems, resultTotal) = await _repo.GetAllAsync(pagination);

        // Assert
        resultItems.Should().BeEquivalentTo(items);
        resultTotal.Should().Be(10);

        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task GetAllAsync_ShouldClampInvalidPagination()
    {
        // Arrange
        var pagination = new PaginationParams(0, 0);

        _dapperMock
            .Setup(d => d.QueryAsync<Example>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(Enumerable.Empty<Example>());

        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.IsAny<string>(), null))
            .ReturnsAsync(new[] { 0 });

        // Act
        var (_, total) = await _repo.GetAllAsync(pagination);

        // Assert
        total.Should().Be(0);
        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task AddAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var entity = new Example("Name", "Desc", new System.DateTime(2025, 1, 1), "Loc", Difficulty.Medium);
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"
                INSERT INTO Examples
                    (Id, Name, Description, Date, Location, Latitude, Longitude, Difficulty, Active, CreatedAt)
                VALUES
                    (@Id, @Name, @Description, @Date, @Location, @Latitude, @Longitude, @Difficulty, 1, GETUTCDATE());";

        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("INSERT INTO Examples")), It.IsAny<DynamicParameters>(), null))
            .Callback<string, object?, IDbTransaction?>((sql, param, transaction) =>
            {
                capturedQuery = sql;
                capturedParams = param as DynamicParameters;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repo.AddAsync(entity);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.ParameterNames.Should().Contain("Id");
        capturedParams.ParameterNames.Should().Contain("Name");
        capturedParams.ParameterNames.Should().Contain("Description");
        capturedParams.ParameterNames.Should().Contain("Date");
        capturedParams.ParameterNames.Should().Contain("Location");
        capturedParams.ParameterNames.Should().Contain("Latitude");
        capturedParams.ParameterNames.Should().Contain("Longitude");
        capturedParams.ParameterNames.Should().Contain("Difficulty");
        capturedParams.ParameterNames.Should().HaveCount(8);
    }
    
    [Fact]
    public async Task AddAsync_ShouldExecuteInsert_AndReturnEntity()
    {
        // Arrange
        var entity = new Example("Name", "Desc", new System.DateTime(2025, 1, 1), "Loc", Difficulty.Medium);

        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("INSERT INTO Examples")), It.IsAny<DynamicParameters>(), null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repo.AddAsync(entity);

        // Assert
        result.Should().BeSameAs(entity);
        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task UpdateAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var entity = new Example("Name", "Desc", new DateTime(2025, 1, 1), "Loc", Difficulty.Medium);
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"
                    UPDATE Examples
                    SET
                        Name = @Name,
                        Description = @Description,
                        Date = @Date,
                        Location = @Location,
                        Latitude = @Latitude,
                        Cost = @Cost,
                        Difficulty = @Difficulty
                    WHERE Id = @Id;";
        
        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("UPDATE Examples")), It.IsAny<DynamicParameters>(), null))
            .Callback<string, object?, IDbTransaction?>((sql, param, transaction) =>
            {
                capturedQuery = sql;
                capturedParams = param as DynamicParameters;
            })
            .Returns(Task.CompletedTask);

        // Act
        await _repo.UpdateAsync(entity);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.ParameterNames.Should().Contain("Id");
        capturedParams.ParameterNames.Should().Contain("Name");
        capturedParams.ParameterNames.Should().Contain("Description");
        capturedParams.ParameterNames.Should().Contain("Date");
        capturedParams.ParameterNames.Should().Contain("Location");
        capturedParams.ParameterNames.Should().Contain("Latitude");
        capturedParams.ParameterNames.Should().Contain("Longitude");
        capturedParams.ParameterNames.Should().Contain("Difficulty");
        capturedParams.ParameterNames.Should().HaveCount(8);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldExecuteUpdate()
    {
        // Arrange
        var entity = new Example("Name", "Desc", new System.DateTime(2025, 1, 1), "Loc", Difficulty.Hard);

        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("UPDATE Examples")), It.IsAny<DynamicParameters>(), null))
            .Returns(Task.CompletedTask);

        // Act
        await _repo.UpdateAsync(entity);

        // Assert
        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task DeleteAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"DELETE FROM Examples WHERE Id = @Id;";

        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("DELETE FROM Examples")), It.IsAny<DynamicParameters>(), null))
            .Callback<string, object?, IDbTransaction?>((sql, param, transaction) =>
            {
                capturedQuery = sql;
                capturedParams = param as DynamicParameters;
            })
            .Returns(Task.CompletedTask);

        // Act
        await _repo.DeleteAsync(id);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.ParameterNames.Should().Contain("Id");
        capturedParams.ParameterNames.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldExecuteDelete()
    {
        // Arrange
        var id = Uuid.CreateVersion7();

        _dapperMock
            .Setup(d => d.ExecuteAsync(It.Is<string>(s => s.Contains("DELETE FROM Examples")), It.IsAny<DynamicParameters>(), null))
            .Returns(Task.CompletedTask);

        // Act
        await _repo.DeleteAsync(id);

        // Assert
        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task ExistsAsync_ShouldBuildQuery_AndBindParams()
    {
        // Arrange
        var id = Uuid.CreateVersion7();
        string capturedQuery = null;
        DynamicParameters capturedParams = null;
        var expectedQuery = @"SELECT 1 FROM Examples WHERE Id = @Id;";

        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.Is<string>(s => s.Contains("SELECT 1 FROM Examples")), It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters?>((sql, dynamicParameters) =>
            {
                capturedQuery = sql;
                capturedParams = dynamicParameters;
            })
            .ReturnsAsync(new[] { 1 });

        // Act
        await _repo.ExistsAsync(id);

        // Assert
        capturedQuery.Should().Be(expectedQuery);
        capturedParams.Should().NotBeNull();
        capturedParams.ParameterNames.Should().Contain("Id");
        capturedParams.ParameterNames.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenAny()
    {
        // Arrange
        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.Is<string>(s => s.Contains("SELECT 1 FROM Examples")), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(new[] { 1 });

        // Act
        var exists = await _repo.ExistsAsync(Uuid.CreateVersion7());

        // Assert
        exists.Should().BeTrue();
        _dapperMock.VerifyAll();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEmpty()
    {
        // Arrange
        _dapperMock
            .Setup(d => d.QueryAsync<int>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .ReturnsAsync(Enumerable.Empty<int>());

        // Act
        var exists = await _repo.ExistsAsync(Uuid.CreateVersion7());

        // Assert
        exists.Should().BeFalse();
        _dapperMock.VerifyAll();
    }
}
