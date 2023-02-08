using Microsoft.Extensions.DependencyInjection;
using BatchDemo.Utility;
using BatchDemo.Utility.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace BatchDemo.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }
    }
}
