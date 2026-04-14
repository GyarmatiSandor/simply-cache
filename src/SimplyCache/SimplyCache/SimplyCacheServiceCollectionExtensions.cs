using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimplyCache
{
    public static class SimplyCacheServiceCollectionExtensions
    {
        public static IServiceCollection UseSimplyCache(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new SimplyCacheOptions();
            configuration.GetSection("SimplyCache").Bind(options);
            services.AddSingleton(options);
            return services;
        }
    }
}
