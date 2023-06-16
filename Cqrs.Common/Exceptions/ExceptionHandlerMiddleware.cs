using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Cqrs.Common.Api;
using Cqrs.Common.Logging;

namespace Cqrs.Common.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingService<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILoggingService<ExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, $"APIExceptionHandlerMiddleware: Error in {context.Request.Path} {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            return exception switch
            {
                HandledException ex => HandleHandledException(context, ex),
                UnauthorizedAccessException ex => HandleNotAuthorisedException(context, ex),
                System.Security.SecurityException ex => HandleNotAuthorisedException(context, ex),
                ValidationException ex => HandleValidationException(context, ex),
                NotFoundException ex => HandleNotFoundException(context, ex),
                DependencyException ex => HandleDependencyServiceException(context, ex),
                _ => HandleUnknownException(context, exception),
            };
        }

        private Task HandleHandledException(HttpContext context, HandledException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;

            var response = new Envelope<object>(exception);

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private Task HandleNotAuthorisedException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var response = new Envelope<object>(new HandledException(
                context.Request.GetEncodedUrl(),
                StatusCodes.Status403Forbidden.ToString(),
                $"You are not authorised to perform the requested action.",
                exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private Task HandleValidationException(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new Envelope<object>(new HandledException(
                context.Request.GetEncodedUrl(),
                StatusCodes.Status400BadRequest.ToString(),
                "A validation error occurred.",
                exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private Task HandleNotFoundException(HttpContext context, NotFoundException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var response = new Envelope<object>(new HandledException(
                context.Request.GetEncodedUrl(),
                StatusCodes.Status404NotFound.ToString(),
                "The specified resource was not found.",
                exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private Task HandleDependencyServiceException(HttpContext context, DependencyException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status424FailedDependency;

            var response = new Envelope<object>(new HandledException(
                exception.Url,
                exception.Code,
                exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private Task HandleUnknownException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new Envelope<object>(new HandledException(
                context.Request.GetEncodedUrl(),
                StatusCodes.Status500InternalServerError.ToString(),
                "An error occurred while processing your request.",
                exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
