using BatchDemo.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
