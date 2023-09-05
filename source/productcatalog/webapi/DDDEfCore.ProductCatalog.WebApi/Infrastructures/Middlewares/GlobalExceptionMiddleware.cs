using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonOptions _jsonOptions;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, IOptions<JsonOptions> jsonOptions)
    {
        this._next = next;
        this._jsonOptions = jsonOptions.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionResponse = new ExceptionResponse();
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        exceptionResponse.ErrorMessages = new List<string> 
        {
            exception.Message
        };

        if (exception is ValidationException validationException)
        {
            var jsonSerializerOptions = this._jsonOptions.SerializerOptions;

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            exceptionResponse.ErrorMessages = JsonSerializer.Deserialize<List<string>>(validationException.Message, jsonSerializerOptions);
        }

        exceptionResponse.Status = context.Response.StatusCode;

        var jsonContent = JsonSerializer.Serialize(exceptionResponse, this._jsonOptions.SerializerOptions);

        await context.Response.WriteAsync(jsonContent);
    }

    public class ExceptionResponse
    {
        public int Status { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
