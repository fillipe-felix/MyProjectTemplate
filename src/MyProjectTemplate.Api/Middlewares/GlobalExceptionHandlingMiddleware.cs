using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

using FluentValidation;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Core.Exceptions;

using Serilog.Context;

namespace MyProjectTemplate.Api.Middlewares;

[ExcludeFromCodeCoverage]
public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            {
                await next(context);
            }
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .Select(e => new BaseError
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                })
                .ToList();
            
            await HandleException(context, (int)HttpStatusCode.BadRequest, String.Empty, errors);
        }
        catch (ErrorException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (ConflictException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (InternalServerErrorException ex)
        {
            await HandleException(context, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleException(context, (int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private async Task HandleException(HttpContext context, int code, string message, List<BaseError>? errors = null)
    {
        var response = errors?.Any() == true ?
            new BaseErrorResult( false, message, errors) :
            new BaseResult(false, message);

        string json = JsonSerializer.Serialize((object)response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;
        
        _logger.LogError("Error Request | StatusCode: {StatusCode} | Message: {Message}", code, message);
        
        await context.Response.WriteAsync(json);
    }
}