namespace SimplyCache
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// Optional cache key for the method result.
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// Cache duration in seconds. If not set or set to 0, the default duration will be used.
        /// </summary>
        public int CacheDuration { get; set; }

        public CacheAttribute(string cacheKey = null, int cacheDuration = 0)
        {
            CacheKey = cacheKey;
            CacheDuration = cacheDuration;
        }
    }
}
