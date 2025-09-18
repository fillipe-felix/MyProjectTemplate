using FluentValidation;

using MediatR;

namespace MyProjectTemplate.Application.Behaviors;

public class ValidationBehavior<IRequest, TResponse> : IPipelineBehavior<IRequest, TResponse> where IRequest : notnull
{

    private readonly IEnumerable<IValidator<IRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<IRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(IRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<IRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors)
                .Where(f => f != null).ToList();  
            
            if (failures.Any())
                throw new ValidationException(failures);
        }
        
        return await next();
    }
}