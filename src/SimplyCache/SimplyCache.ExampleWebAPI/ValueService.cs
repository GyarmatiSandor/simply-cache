namespace SimplyCache.ExampleWebAPI
{
    public class ValueService : IValueService
    {
        [Cache]
        public int GetValue(int input)
        {
            return input * 2;
        }

        [Cache(CacheDuration = 10)]
        public int GetValueWithExpiration(int input)
        {
            return input * 3;
        }
    }
}
