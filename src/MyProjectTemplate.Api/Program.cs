using MyProjectTemplate.Api.Configuration;
using MyProjectTemplate.Api.Middlewares;
using MyProjectTemplate.Application;
using MyProjectTemplate.Infra;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

IConfiguration configuration = builder.Configuration;

builder.Services
    .AddPresentation()
    .configureAppSettings(configuration)
    .AddApplication()
    .AddInfra()
    .AddMiddlewares();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.Run();
