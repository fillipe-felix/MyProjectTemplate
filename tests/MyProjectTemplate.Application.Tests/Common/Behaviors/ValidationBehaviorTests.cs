using FluentAssertions;

using FluentValidation;

using MediatR;

using MyProjectTemplate.Application.Common.Behaviors;

using Xunit;

namespace MyProjectTemplate.Application.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.")
                .MinimumLength(3).WithMessage("Value must be at least 3 characters.");
        }
    }

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, string>(Array.Empty<IValidator<TestRequest>>());
        var wasCalled = false;

        Task<string> Next(CancellationToken cancellationToken)
        {
            wasCalled = true;
            return Task.FromResult("ok");
        }

        RequestHandlerDelegate<string> next = Next;

        // Act
        var result = await behavior.Handle(new TestRequest("x"), next, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldCallNext_AndReturnResult()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        Task<string> Next(CancellationToken cancellationToken) => Task.FromResult("success");
        RequestHandlerDelegate<string> next = Next;

        // Act
        var result = await behavior.Handle(new TestRequest("valid"), next, CancellationToken.None);

        // Assert
        result.Should().Be("success");
    }

    [Fact]
    public async Task Handle_WhenInvalid_ShouldThrowValidationException()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        Task<string> Next(CancellationToken cancellationToken) => Task.FromResult("should-not-reach");
        RequestHandlerDelegate<string> next = Next;

        // Act
        var act = async () => await behavior.Handle(new TestRequest(""), next, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage == "Value is required.");
    }

    [Fact]
    public async Task Handle_WhenMultipleValidatorsWithFailures_ShouldAggregateAndThrow()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        Task<string> Next(CancellationToken cancellationToken) => Task.FromResult("should-not-reach");
        RequestHandlerDelegate<string> next = Next;

        // Act
        var act = async () => await behavior.Handle(new TestRequest(""), next, CancellationToken.None);

        // Assert
        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage == "Value must be at least 3 characters.");
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage == "Value is required.");
    }
}
