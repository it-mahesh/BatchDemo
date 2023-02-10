using BatchDemo.Utility.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [ExcludeFromCodeCoverage]
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _correlationIdHeader = "X-Correlation-Id";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationIdGenerator"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            var correlationId = GetCorrelationId(context, correlationIdGenerator);
            AddCorrelationIdHeaderToResponse(context, correlationId);

            await _next(context);
        }
        private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId))
            {
                correlationIdGenerator.Set(correlationId!);
                return correlationId;
            }
            else
            {
                return correlationIdGenerator.Get();
            }
        }
        private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
        => context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(_correlationIdHeader, new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });
    }
}
