# DispatchProxy in .NET: Overview and Caching Use Case

## What is DispatchProxy?

`DispatchProxy` is a class in .NET that allows you to create dynamic proxy objects for interfaces at runtime. It enables you to intercept method calls and add custom logic before or after the actual method execution, similar to how proxies work in other languages (like Java's dynamic proxies).

---

## How Does DispatchProxy Work?

- You define a class that inherits from `DispatchProxy` and implements the `Invoke` method.
- At runtime, you create a proxy instance for a given interface using `DispatchProxy.Create<T, TProxy>()`.
- All calls to the interface methods are routed through your `Invoke` method, where you can add custom logic (e.g., logging, caching, validation).

**Key Points:**
- Only works with interfaces (not concrete classes).
- Uses reflection and dynamic code generation under the hood.
- Available in .NET Core and .NET 5+.

---

## What is DispatchProxy Good For?

- **Cross-cutting concerns:** Logging, caching, validation, authorization, etc.
- **Aspect-Oriented Programming:** Add behaviors without modifying the original code.
- **Testing and mocking:** Create test doubles or interceptors for interfaces.

---

## Using DispatchProxy for Caching

You can use `DispatchProxy` to create a generic caching proxy for any interface. The proxy intercepts method calls, checks if the result is cached, and only calls the real implementation if needed.

### Example: Caching Proxy Implementation

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
        var cacheKey = $"{targetMethod.Name}:{string.Join("_", args)}";
        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var result = targetMethod.Invoke(_decorated, args);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
```

### How to Use

```csharp
// Assume you have an interface IMyService and a real implementation MyService
var proxy = DispatchProxy.Create<IMyService, CachingProxy<IMyService>>();
((CachingProxy<IMyService>)proxy).SetParameters(realService, memoryCache);

// Use 'proxy' as your service instance; method calls will be cached automatically.
```

---

## Pros and Cons

**Pros:**
- No need to write a proxy for each interface.
- Centralizes caching logic.
- Works for any interface-based service.

**Cons:**
- Only works with interfaces.
- Some performance overhead due to reflection.
- Does not support async methods out-of-the-box (requires extra handling).

---

## Summary

`DispatchProxy` is a powerful tool for adding cross-cutting concerns like caching to interface-based services in .NET. It enables you to create reusable, generic proxies with minimal boilerplate, making it a great fit for library authors and advanced scenarios.

---

**Further Reading:**
- [Microsoft Docs: DispatchProxy](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy)
- [Caching in .NET](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/)
