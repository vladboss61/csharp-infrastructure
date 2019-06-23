using System;
using CSharp.Infrastructure.Interfaces;

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

        public T Item
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(Guard<T>));
                }

                return _item;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pool.Return(_item);
                _disposed = true;
                return;
            }

            if (!_disposed)
            {
                _pool.Return(_item);
            }
            _disposed = true;
        }

        ~Guard()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
