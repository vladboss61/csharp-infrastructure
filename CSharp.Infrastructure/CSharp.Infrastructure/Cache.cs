using System;
using System.Collections.Generic;
using System.Linq;
using CSharp.Infrastructure.Extensions;
using CSharp.Infrastructure.Interfaces;

namespace CSharp.Infrastructure
{
    public class Cache : ICache
    {
        private readonly Dictionary<Type, IList<object>> _cache;

        public Cache()
        {
            _cache = new Dictionary<Type, IList<object>>();
        }

        public void Put<T>(T instance)
        {
            var type = typeof(T);
            if (_cache.ContainsKey(type))
            {
                _cache[type].Add(instance);
            }
            else
            {
                var sequence = new List<object>() { instance };
                _cache.Add(type, sequence);
            }
        }

        public T Get<T>(Func<T, bool> predicate) =>
            _cache.TryGetValue(typeof(T), out var instance) ? instance.Cast<T>().First(predicate)
                : throw new InvalidOperationException($"Does not know about {typeof(T).Name}");

        public List<T> Get<T>() =>
            _cache.TryGetValue(typeof(T), out var instance) ? instance.Cast<T>().OrEmpty().ToList()
                : throw new InvalidOperationException($"Does not know about {typeof(T).Name}");
    }
}
