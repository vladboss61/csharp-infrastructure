using System;

namespace CSharp.Infrastructure
{
    public class Guard<T> : IDisposable where T : class
    {
        private readonly IPool<T> _pool;
        private readonly T _item;
        private bool _disposed;

        public Guard(T item, IPool<T> pool)
        {
            _pool = pool;
            _item = item;
            _disposed = false;
        }

        T Item
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(_item));
                }

                return _item;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _pool.Return(_item);
            }
            _disposed = true;
        }
    }
}
