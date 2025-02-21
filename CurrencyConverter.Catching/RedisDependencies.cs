using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Catching
{
    public static class RedisDependencies
    {
        public static IServiceCollection AddRedisCacheProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheProvider, RedisCacheProvider>();
            return services;
        }

    }
}
