using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using MyProjectTemplate.Domain.Interfaces;
using MyProjectTemplate.Infra.Repositories;

namespace MyProjectTemplate.Infra;

[ExcludeFromCodeCoverage]
public static class InfraModule
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IExampleRepository, ExampleRepository>();
        return services;
    }
}
