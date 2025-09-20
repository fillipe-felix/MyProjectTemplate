using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
#if (UseEFSqlServer || UseEFPostgres)
using MyProjectTemplate.Domain.Interfaces;
using MyProjectTemplate.Infra.Repositories;
#endif
#if (UseDapperSqlServer || UseDapperPostgres)
using MyProjectTemplate.Infra.Adapters;
using MyProjectTemplate.Infra.Contracts;
#endif

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
#if (UseEFSqlServer || UseEFPostgres)
        services.AddScoped<IExampleRepository, ExampleRepository>();
#endif
#if (UseDapperSqlServer || UseDapperPostgres)  
        services.AddScoped<IDapperWrapper, DapperWrapper>();
#endif
        return services;
    }
}
