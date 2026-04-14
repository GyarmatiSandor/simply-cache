using Microsoft.Extensions.Configuration;

namespace SimplyCache
{
    public class SimplyCacheOptions
    {
        public int DefaultCacheDurationInSeconds { get; set; } = 60;
    }
}
