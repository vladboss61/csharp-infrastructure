using System;
using System.Collections.Concurrent;
using CSharp.Infrastructure.Interfaces;

namespace CSharp.Infrastructure
{
    public class Pool<T> : IPool<T> where T : class
    {
        private static readonly object sync = new object();
        private readonly ConcurrentQueue<T> _bag;
        private readonly Func<T> _factory;
        private readonly T _item;

        public Pool(Func<T> factory)
        {
            _factory = factory;
            _bag = new ConcurrentQueue<T>();
            _item = null;
        }

        public Pool(T item)
        {
            _item = item;
            _bag = new ConcurrentQueue<T>();
            _factory = null;
        }

        public Guard<T> Rent()
        {
            if (_bag.TryDequeue(out var item))
            {
                return new Guard<T>(item, this);
            }

            lock (sync)
            {
                return _item != null 
                    ? new Guard<T>(_item, this) 
                    : new Guard<T>(_factory(), this);
            }
        }

        public void Return(T item)
        {
            _bag.Enqueue(item);
        }
    }
}
