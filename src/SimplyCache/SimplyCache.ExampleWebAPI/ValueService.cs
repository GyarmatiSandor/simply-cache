namespace SimplyCache.ExampleWebAPI
{
    public class ValueService : IValueService
    {
        [Cache]
        public int GetValue(int input)
        {
            return input * 2;
        }
    }
}
