using System.Reflection;

namespace SimplyCache
{
    internal class SimplyCacheDispatchProxy<T> : DispatchProxy
        where T : class
    {
        private T _decorated;
        private IInMemoryCacheService _cache;

        public void SetParameters(T decorated, IInMemoryCacheService cache)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));

            // Check for attribute on interface method
            var useCache = targetMethod.GetCustomAttribute<CacheAttribute>() != null;

            // If not found, check on implementation method
            if (!useCache)
            {
                var interfaceMap = _decorated.GetType().GetInterfaceMap(typeof(T));
                int methodIndex = Array.IndexOf(interfaceMap.InterfaceMethods, targetMethod);
                if (methodIndex >= 0)
                {
                    var implMethod = interfaceMap.TargetMethods[methodIndex];
                    useCache = implMethod.GetCustomAttribute<CacheAttribute>() != null;
                }
            }

            if (!useCache)
                return targetMethod.Invoke(_decorated, args);

            var cacheKey = $"{typeof(T).FullName}:{targetMethod.Name}:{string.Join("_", args ?? Array.Empty<object>())}";

            var returnType = targetMethod.ReturnType;
            var tryGetValueMethod = typeof(IInMemoryCacheService).GetMethod("TryGetValue").MakeGenericMethod(returnType);
            var parameters = new object[] { cacheKey, null };
            var found = (bool)tryGetValueMethod.Invoke(_cache, parameters);
            if (found)
            {
                return parameters[1];
            }

            var result = targetMethod.Invoke(_decorated, args);
            var setMethod = typeof(IInMemoryCacheService).GetMethod("Set").MakeGenericMethod(returnType);
            setMethod.Invoke(_cache, new object[] { cacheKey, result, null });
            return result;
        }
    }
}