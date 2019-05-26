using System;
using System.Collections.Concurrent;
using CSharp.Infrastructure.Interfaces;

namespace CSharp.Infrastructure
{
    public class MemorySettings : ISettings
    {
        private readonly ConcurrentDictionary<Type, object> _settings;

        public MemorySettings()
        {
            _settings = new ConcurrentDictionary<Type, object>();
        }

        public void Put<TSetting>(TSetting setting)
        {
            var type = typeof(TSetting);
            if (!_settings.TryAdd(type, setting))
            {
                throw new InvalidOperationException($"Try to duplicate a setting : {type.Name}.");
            }
        }

        public TSetting Obtain<TSetting>()
        {
            var type = typeof(TSetting);
            if (_settings.TryGetValue(type, out var setting))
            {
                return (TSetting) setting;
            }
            throw new InvalidOperationException($"Try to obtain the unknown setting : {type.Name}.");
        }
    }
}
