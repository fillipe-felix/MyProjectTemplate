using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using MyProjectTemplate.Api.Middlewares;
using MyProjectTemplate.Infra;
using MyProjectTemplate.Infra.Data;

namespace MyProjectTemplate.Api.Configuration;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PedalPoint", Version = "v1" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection configureAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, true)
            .Build();
#endif

        var dbRepositoryAdapterConfiguration = configuration.GetSection("DbRepositoryAdapterConfiguration")
            .Get<DbRepositoryAdapterConfiguration>();

        services.AddSingleton<DbRepositoryAdapterConfiguration>(dbRepositoryAdapterConfiguration);

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbRepositoryAdapterConfiguration.SqlConnectionString));

        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<GlobalExceptionHandlingMiddleware>();

        return services;
    }
}