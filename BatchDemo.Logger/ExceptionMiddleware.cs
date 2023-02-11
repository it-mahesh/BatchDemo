using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BatchDemo.Logger
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.Core.Logger _logger;
        public ExceptionMiddleware(RequestDelegate next, Serilog.Core.Logger logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(httpContext, ex,_logger);
            }
        }

        private static Task HandleGlobalExceptionAsync(HttpContext context, Exception exception, Serilog.Core.Logger logger)
        {
            //Technical Exception for troubleshooting
            //logger.Error(exception.Message);
            logger.Error(exception, "Something went wrong.");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // Business exception for users with gracefull exit message.
            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Something went wrong !Internal Server Error"
            }.ToString());
        }
    }
}