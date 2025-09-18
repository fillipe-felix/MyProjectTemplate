using FluentAssertions;

using FluentValidation.TestHelper;

using MyProjectTemplate.Application.Example.Commands.CreateExample;
using MyProjectTemplate.Domain.Enums;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Example.Commands.CreateExample;

public class CreateExampleCommandValidatorTests
{
    private readonly CreateExampleCommandValidator _validator = new();

    private static CreateExampleCommand ValidCommand => new()
    {
        Name = "Valid Name-123",
        Description = "Some description",
        Date = DateTime.UtcNow.Date.AddDays(1),
        Location = "Valid Location_1",
        Latitude = null,
        Longitude = null,
        Difficulty = Difficulty.Medium
    };

    [Fact]
    public void ValidCommand_ShouldPassValidation()
    {
        var model = ValidCommand;

        var result = _validator.TestValidate(model);

        result.IsValid.Should().BeTrue(result.ToString());
    }

    [Fact]
    public void Name_ShouldBeRequired_AndWithinLength_AndMatchRegex()
    {
        var model = ValidCommand with { Name = "" };

        var r1 = _validator.TestValidate(model);
        r1.ShouldHaveValidationErrorFor(x => x.Name)
          .WithErrorMessage("Name is required.");

        model = model with { Name = "ab" };
        var r2 = _validator.TestValidate(model);
        r2.ShouldHaveValidationErrorFor(x => x.Name)
          .WithErrorMessage("Name must be at least 3 characters long.");

        model = model with { Name = new string('x', 101) };
        var r3 = _validator.TestValidate(model);
        r3.ShouldHaveValidationErrorFor(x => x.Name)
          .WithErrorMessage("Name must be at most 100 characters long.");

        model = model with { Name = "Invalid@Name!" };
        var r4 = _validator.TestValidate(model);
        r4.ShouldHaveValidationErrorFor(x => x.Name)
          .WithErrorMessage("Name contains invalid characters.");
    }

    [Fact]
    public void Description_ShouldHaveMaxLength()
    {
        var model = ValidCommand with { Description = new string('d', 501) };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description must be at most 500 characters long.");
    }

    [Fact]
    public void Date_ShouldNotBeInPast()
    {
        var model = ValidCommand with { Date = DateTime.UtcNow.Date.AddDays(-1) };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Date)
              .WithErrorMessage("Date cannot be in the past.");
    }

    [Fact]
    public void Location_ShouldBeRequired_WithinLength_AndMatchRegex()
    {
        var model = ValidCommand with { Location = "" };

        var r1 = _validator.TestValidate(model);
        r1.ShouldHaveValidationErrorFor(x => x.Location)
          .WithErrorMessage("Location is required.");

        model = model with { Location = "A" };
        var r2 = _validator.TestValidate(model);
        r2.ShouldHaveValidationErrorFor(x => x.Location)
          .WithErrorMessage("Location must be at least 2 characters long.");

        model = model with { Location = new string('x', 201) };
        var r3 = _validator.TestValidate(model);
        r3.ShouldHaveValidationErrorFor(x => x.Location)
          .WithErrorMessage("Location must be at most 200 characters long.");

        model = model with { Location = "Invalid@Location!" };
        var r4 = _validator.TestValidate(model);
        r4.ShouldHaveValidationErrorFor(x => x.Location)
          .WithErrorMessage("Location contains invalid characters.");
    }

    [Fact]
    public void Coordinates_ShouldBeProvidedTogether_OrBothNull()
    {
        var onlyLat = ValidCommand with { Latitude = 10m, Longitude = null };
        var r1 = _validator.TestValidate(onlyLat);
        r1.ShouldHaveValidationErrorFor(x => x.Latitude)
          .WithErrorMessage("Latitude and Longitude must both be provided together or both be empty.");

        var onlyLon = ValidCommand with { Latitude = null, Longitude = 20m };
        var r2 = _validator.TestValidate(onlyLon);
        r2.ShouldHaveValidationErrorFor(x => x.Longitude)
          .WithErrorMessage("Latitude and Longitude must both be provided together or both be empty.");
    }

    [Theory]
    [InlineData(-91, 0, "Latitude must be between -90 and 90.")]
    [InlineData(91, 0, "Latitude must be between -90 and 90.")]
    [InlineData(0, -181, "Longitude must be between -180 and 180.")]
    [InlineData(0, 181, "Longitude must be between -180 and 180.")]
    public void Coordinates_OutOfRange_ShouldFail(decimal lat, decimal lon, string expectedMessage)
    {
        var model = ValidCommand with { Latitude = lat, Longitude = lon };

        var result = _validator.TestValidate(model);

        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();
        messages.Should().Contain(expectedMessage);
    }

    [Fact]
    public void Difficulty_ShouldBeInEnum()
    {
        var model = ValidCommand;
        // Force an invalid enum by casting an int outside defined range
        var invalidDifficulty = (Difficulty)999;
        model.Difficulty = invalidDifficulty;

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Difficulty)
              .WithErrorMessage("Difficulty has an invalid value.");
    }
}
