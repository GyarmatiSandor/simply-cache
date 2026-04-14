using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SimplyCache
{
    public static class SimplyCacheInjection
    {
        public static void AddCachedSingleton<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddInMemoryCache();
            services.AddSingleton<TInterface>(provider =>
            {
                var implementation = Activator.CreateInstance<TImplementation>();
                var cacheService = provider.GetRequiredService<IInMemoryCacheService>();
                return CreateCachedInstance<TInterface>(implementation, cacheService);
            });
        }

        public static void AddCachedScoped<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddInMemoryCache();
            services.AddScoped<TInterface>(provider =>
            {
                var implementation = Activator.CreateInstance<TImplementation>();
                var cacheService = provider.GetRequiredService<IInMemoryCacheService>();
                return CreateCachedInstance<TInterface>(implementation, cacheService);
            });
        }

        public static void AddCachedTransient<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddInMemoryCache();
            services.AddTransient<TInterface>(provider =>
            {
                var implementation = Activator.CreateInstance<TImplementation>();
                var cacheService = provider.GetRequiredService<IInMemoryCacheService>();
                return CreateCachedInstance<TInterface>(implementation, cacheService);
            });
        }

        private static void AddInMemoryCache(this IServiceCollection services)
        {
            services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
        }

        private static TInterface CreateCachedInstance<TInterface>(object decorated, IInMemoryCacheService cache) where TInterface : class
        {
            var proxy = DispatchProxy.Create<TInterface, SimplyCacheDispatchProxy<TInterface>>() as SimplyCacheDispatchProxy<TInterface>;
            proxy.SetParameters((TInterface)decorated, cache);
            return proxy as TInterface;
        }
    }
}
