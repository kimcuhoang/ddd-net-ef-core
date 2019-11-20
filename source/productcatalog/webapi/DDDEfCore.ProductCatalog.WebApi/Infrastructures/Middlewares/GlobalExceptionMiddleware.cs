using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            this._next = next;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionResponse = new ExceptionResponse();
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            exceptionResponse.ErrorMessages = new List<string> {exception.Message};

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                exceptionResponse.ErrorMessages = validationException.Errors.Select(x => x.ErrorMessage).ToList();
            }

            exceptionResponse.Status = context.Response.StatusCode;
            var jsonContent = JsonSerializer.Serialize(new { response = exceptionResponse },
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return context.Response.WriteAsync(jsonContent);
        }

        public class ExceptionResponse
        {
            public int Status { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }
    }
}
