using System.Net;
using System.Text.Json;
using AcervoEducacional.Application.DTOs.Common;

namespace AcervoEducacional.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>();

        switch (exception)
        {
            case ArgumentException argEx:
                response = ApiResponse<object>.ErrorResult(argEx.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                response = ApiResponse<object>.ErrorResult("Acesso não autorizado");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                response = ApiResponse<object>.ErrorResult("Recurso não encontrado");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidOperationException invOpEx:
                response = ApiResponse<object>.ErrorResult(invOpEx.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case TimeoutException:
                response = ApiResponse<object>.ErrorResult("Operação expirou. Tente novamente.");
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                response = ApiResponse<object>.ErrorResult("Erro interno do servidor");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        
        // Log da requisição
        _logger.LogInformation("Iniciando {Method} {Path} de {RemoteIpAddress}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        await _next(context);

        var duration = DateTime.UtcNow - startTime;

        // Log da resposta
        _logger.LogInformation("Concluído {Method} {Path} com {StatusCode} em {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}

