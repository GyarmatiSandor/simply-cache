using Microsoft.OpenApi;

namespace SimplyCache.ExampleWebAPI
{
    public interface IValueService
    {
        int GetValue(int input);
        int GetValueWithExpiration(int input);
    }
}
