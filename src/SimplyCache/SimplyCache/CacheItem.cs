namespace SimplyCache
{
    public sealed record CacheItem
    {
        public required Type Type { get; init; }
        public required object Value { get; init; }
        public required DateTime Expiration { get; init; }
    }
}
