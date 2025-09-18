using AutoFixture;

using Dodo.Primitives;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

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
    public async Task GetAllAsync_ShouldReturnAll()
    {
        // Arrange
        await _repo.AddAsync(_fixture.Create<Example>());
        await _repo.AddAsync(_fixture.Create<Example>());

        // Act
        var all = await _repo.GetAllAsync();

        // Assert
        all.Should().HaveCount(2);
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
