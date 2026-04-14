# Setting Up Caching with DispatchProxy and Dependency Injection in .NET

This guide describes how to implement a generic caching solution using DispatchProxy and integrate it with Dependency Injection (DI) in .NET. The goal is to allow consumers to transparently receive a caching proxy for their services, following an agentic (automated, reusable) approach.

---

## 1. Define a Cache Attribute

Create a custom attribute to mark methods that should be cached:

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute : Attribute { }
```

---

## 2. Implement a Generic CachingProxy

Inherit from DispatchProxy and implement caching logic that checks for the attribute:

```csharp
public class CachingProxy<T> : DispatchProxy
{
    private T _decorated;
    private IMemoryCache _cache;

    public void SetParameters(T decorated, IMemoryCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        var useCache = targetMethod.GetCustomAttribute<CacheAttribute>() != null;
        if (!useCache)
            return targetMethod.Invoke(_decorated, args);

        var cacheKey = $"{targetMethod.Name}:{string.Join("_", args)}";
        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var result = targetMethod.Invoke(_decorated, args);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
```

---

## 3. Register Services with DI to Use the Proxy

Register the real implementation and then the interface with a factory that returns the proxy:

```csharp
services.AddSingleton<MyService>(); // Real implementation
services.AddSingleton<IMyService>(provider =>
{
    var real = provider.GetRequiredService<MyService>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    var proxy = DispatchProxy.Create<IMyService, CachingProxy<IMyService>>();
    ((CachingProxy<IMyService>)proxy).SetParameters(real, cache);
    return proxy;
});
```

- Consumers inject `IMyService` and receive the proxy with caching logic.
- You can generalize this pattern for multiple services using extension methods or reflection.

---

## 4. Usage Example

```csharp
public interface IMyService
{
    [Cache]
    int GetValue(int id);
}

public class MyService : IMyService
{
    public int GetValue(int id) => /* expensive operation */;
}

// In your controller or consumer:
public class MyController
{
    private readonly IMyService _service;
    public MyController(IMyService service) { _service = service; }
    // Calls to _service.GetValue will be cached if [Cache] is present
}
```

---

## 5. Notes and Extensions

- This approach only works for interfaces.
- For async methods, you need to handle Task return types in the proxy.
- You can extend the proxy to support cache duration, cache key customization, etc.
- Consider providing extension methods to automate proxy registration for multiple services.

---

## Summary

By combining DispatchProxy, custom attributes, and DI registration, you can create a flexible, reusable caching library that is easy for consumers to use and extend. The agentic approach allows for automated setup and minimal manual intervention.

---

**Further Reading:**
- [DispatchProxy Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy)
- [Microsoft Docs: Dependency Injection in .NET](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
