using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using MyProjectTemplate.Application.Common.Behaviors;

namespace MyProjectTemplate.Application;

[ExcludeFromCodeCoverage]
public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationModule));
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining(typeof(ApplicationModule)));

        return services;
    }
}
