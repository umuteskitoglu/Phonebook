using System.Net;
using System.Text.Json;
using Application.Features.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ReportServiceAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        title = "Validation Error",
                        status = statusCode,
                        errors = validationException.Errors
                    });
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    result = JsonSerializer.Serialize(new
                    {
                        title = "Server Error",
                        status = statusCode,
                        detail = exception.Message
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result);
        }
    }
} 