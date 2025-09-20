using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

#if (UseEFSqlServer || UseEFPostgres)
using Microsoft.EntityFrameworkCore;
using MyProjectTemplate.Infra.Data;
#endif

#if UseDapperSqlServer
using System.Data;
using Microsoft.Data.SqlClient;
#endif

#if UseDapperPostgres
using System.Data;
using Npgsql;
#endif

using Microsoft.OpenApi.Models;

using MyProjectTemplate.Api.Middlewares;
using MyProjectTemplate.Infra;

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProjectTemplate", Version = "v1" });

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
        
        #if UseEFSqlServer
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(dbRepositoryAdapterConfiguration.SqlConnectionString));
        #endif

        #if UseEFPostgres
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbRepositoryAdapterConfiguration.SqlConnectionString));
        #endif

        #if UseDapperSqlServer
        services.AddScoped<IDbConnection>(sp =>
        {
            string connString = dbRepositoryAdapterConfiguration.SqlConnectionString;
            return new SqlConnection(connString);
        });
        #endif

        #if UseDapperPostgres
        services.AddScoped<IDbConnection>(sp =>
        {
            string connString = dbRepositoryAdapterConfiguration.SqlConnectionString;
            return new NpgsqlConnection(connString);
        });
        #endif

        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<GlobalExceptionHandlingMiddleware>();

        return services;
    }
}