using System.Linq.Expressions;

using AutoFixture;

using Dodo.Primitives;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Entities;
using MyProjectTemplate.Domain.Enums;
using MyProjectTemplate.Infra.Data;
using MyProjectTemplate.Infra.Repositories;

using Xunit;

namespace MyProjectTemplate.Infra.Tests.Repositories;

public class ExampleRepositoryTests
{
    private readonly AppDbContext _ctx;
    private readonly ExampleRepository _repo;
    private readonly Fixture _fixture;

    public ExampleRepositoryTests()
    {
        _ctx = CreateInMemoryContext();
        _repo = new ExampleRepository(_ctx);
        _fixture = new Fixture();   
    }
    
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"AppDb_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Add_GetById_Update_Delete_Flow_ShouldWork()
    {
        // Arrange
        var entity = _fixture.Create<Example>();

        // Add
        var saved = await _repo.AddAsync(entity);
        saved.Id.Should().NotBe(Uuid.Empty);

        // GetById
        var loaded = await _repo.GetByIdAsync(saved.Id);
        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be(entity.Name);

        // Update
        loaded.Update("B", "New", new DateTime(2026, 2, 2), "NewLoc", 9.9m, 8.8m, Difficulty.Hard);
        await _repo.UpdateAsync(loaded);

        var reloaded = await _repo.GetByIdAsync(saved.Id);
        reloaded!.Name.Should().Be(loaded.Name);
        reloaded.Difficulty.Should().Be(Difficulty.Hard);

        // Exists
        (await _repo.ExistsAsync(saved.Id)).Should().BeTrue();

        // Delete
        await _repo.DeleteAsync(saved.Id);
        (await _repo.GetByIdAsync(saved.Id)).Should().BeNull();
        (await _repo.ExistsAsync(saved.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_WithPaginationAndFilterAndOrder_ShouldReturnPagedItemsAndTotal()
    {
        // Arrange
        var e1 = _fixture.Create<Example>(); // Name random
        var e2 = new Example("Alpha", "D2", DateTime.UtcNow.Date.AddDays(1), "L2", Difficulty.Easy);
        var e3 = new Example("Beta", "D3", DateTime.UtcNow.Date.AddDays(2), "L3", Difficulty.Medium);
        var e4 = new Example("Gamma", "D4", DateTime.UtcNow.Date.AddDays(3), "L4", Difficulty.Hard);

        await _repo.AddAsync(e1);
        await _repo.AddAsync(e2);
        await _repo.AddAsync(e3);
        await _repo.AddAsync(e4);

        var pagination = new PaginationParams(pageNumber: 2, pageSize: 2);
        Expression<Func<Example, bool>> filter = x => x.Name == "Alpha" || x.Name == "Beta" || x.Name == "Gamma";
        Func<IQueryable<Example>, IOrderedQueryable<Example>> orderBy = q => q.OrderBy(x => x.Name);

        // Act
        var (items, total) = await _repo.GetAllAsync(pagination, filter, orderBy, CancellationToken.None);

        // Assert
        total.Should().Be(3);  
        items.Should().HaveCount(1);
        items.First().Name.Should().Be("Gamma");
    }

    [Fact]
    public async Task GetAllAsync_DefaultOrder_WhenNoOrderByProvided_ShouldReturnPagedItemsAndTotal()
    {
        // Arrange
        await _repo.AddAsync(new Example("A", "D", DateTime.UtcNow.Date.AddDays(1), "L", Difficulty.Easy));
        await _repo.AddAsync(new Example("B", "D", DateTime.UtcNow.Date.AddDays(2), "L", Difficulty.Medium));
        await _repo.AddAsync(new Example("C", "D", DateTime.UtcNow.Date.AddDays(3), "L", Difficulty.Hard));

        var pagination = new PaginationParams(pageNumber: 1, pageSize: 2);

        // Act
        var (items, total) = await _repo.GetAllAsync(pagination, cancellationToken: CancellationToken.None);

        // Assert
        total.Should().Be(3);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenIdNotFound()
    {
        // Arrange
        var id = Uuid.CreateVersion7();

        // Act
        var act = async () => await _repo.DeleteAsync(id);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
