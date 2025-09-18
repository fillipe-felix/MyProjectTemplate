using FluentAssertions;

using MyProjectTemplate.Domain.Entities;
using MyProjectTemplate.Domain.Enums;

using Xunit;

namespace MyProjectTemplate.Domain.Tests.Entities;

public class ExampleTests
{
    [Fact]
    public void Constructor_ShouldInitialize_AllProperties()
    {
        // Arrange
        var name = "Name";
        var description = "Desc";
        var date = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var location = "Loc";
        var difficulty = Difficulty.Medium;
        decimal? lat = 1.23m;
        decimal? lng = 4.56m;

        // Act
        var entity = new Example(name, description, date, location, difficulty, lat, lng);

        // Assert
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.Date.Should().Be(date);
        entity.Location.Should().Be(location);
        entity.Difficulty.Should().Be(difficulty);
        entity.Latitude.Should().Be(lat);
        entity.Longitude.Should().Be(lng);
    }

    [Fact]
    public void Constructor_ShouldAllow_NullCoordinates()
    {
        // Act
        var entity = new Example("N", "D", new DateTime(2025, 1, 1), "L", Difficulty.Easy);

        // Assert
        entity.Latitude.Should().BeNull();
        entity.Longitude.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldChange_AllFields()
    {
        // Arrange
        var entity = new Example("N", "D", new DateTime(2025, 1, 1), "L", Difficulty.Easy, 0m, 0m);

        var newName = "New N";
        var newDesc = "New D";
        var newDate = new DateTime(2026, 2, 2, 12, 30, 0, DateTimeKind.Utc);
        var newLoc = "New L";
        decimal? newLat = 10.1m;
        decimal? newLng = 20.2m;
        var newDiff = Difficulty.Hard;

        // Act
        entity.Update(newName, newDesc, newDate, newLoc, newLat, newLng, newDiff);

        // Assert
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDesc);
        entity.Date.Should().Be(newDate);
        entity.Location.Should().Be(newLoc);
        entity.Latitude.Should().Be(newLat);
        entity.Longitude.Should().Be(newLng);
        entity.Difficulty.Should().Be(newDiff);
    }

    [Fact]
    public void Update_ShouldAllow_NullCoordinates()
    {
        // Arrange
        var entity = new Example("N", "D", new DateTime(2025, 1, 1), "L", Difficulty.Medium, 12m, 34m);

        // Act
        entity.Update("X", "Y", new DateTime(2027, 3, 3), "Z", null, null, Difficulty.Easy);

        // Assert
        entity.Latitude.Should().BeNull();
        entity.Longitude.Should().BeNull();
    }
}
