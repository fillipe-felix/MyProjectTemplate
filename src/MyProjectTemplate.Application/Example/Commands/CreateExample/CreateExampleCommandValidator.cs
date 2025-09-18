using System.Text.RegularExpressions;

using FluentValidation;

namespace MyProjectTemplate.Application.Example.Commands.CreateExample;

public class CreateExampleCommandValidator : AbstractValidator<CreateExampleCommand>
{
    public CreateExampleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters long.")
            .Must(name => NameRegex.IsMatch(name)).WithMessage("Name contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters long.");

        RuleFor(x => x.Date)
            .Must(d => d >= DateTime.UtcNow.Date).WithMessage("Date cannot be in the past.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MinimumLength(2).WithMessage("Location must be at least 2 characters long.")
            .MaximumLength(200).WithMessage("Location must be at most 200 characters long.")
            .Must(loc => LocationRegex.IsMatch(loc)).WithMessage("Location contains invalid characters.");

        RuleFor(x => x.Latitude)
            .Cascade(CascadeMode.Stop)
            .Must((cmd, lat) => CoordinatesProvidedTogether(cmd)).WithMessage("Latitude and Longitude must both be provided together or both be empty.")
            .When(x => x.Longitude is not null || x.Latitude is not null)
            .Must(lat => lat is null || (lat >= -90m && lat <= 90m)).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .Cascade(CascadeMode.Stop)
            .Must((cmd, lon) => CoordinatesProvidedTogether(cmd)).WithMessage("Latitude and Longitude must both be provided together or both be empty.")
            .When(x => x.Longitude is not null || x.Latitude is not null)
            .Must(lon => lon is null || (lon >= -180m && lon <= 180m)).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Difficulty has an invalid value.");

    }
    
    private static bool CoordinatesProvidedTogether(CreateExampleCommand cmd) =>
        (cmd.Latitude is null && cmd.Longitude is null) ||
        (cmd.Latitude is not null && cmd.Longitude is not null);

    private static readonly Regex NameRegex =
        new(@"^[\p{L}\p{N} .,'_-]+$", RegexOptions.Compiled);

    private static readonly Regex LocationRegex =
        new(@"^[\p{L}\p{N} .,'_-]+$", RegexOptions.Compiled);

}
