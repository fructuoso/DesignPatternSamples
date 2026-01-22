using System.Net;
using System.Text.Json;
using DesignPatternSamples.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DesignPatternSamples.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context);
        }
    }

    private Task HandleExceptionAsync(HttpContext context)
    {
        var code = HttpStatusCode.InternalServerError;

        string result = JsonSerializer.Serialize(new FailureResultModel("Ocorreu um erro inesperado"));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
