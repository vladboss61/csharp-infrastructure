using System.Collections.Generic;

namespace CSharp.Infrastructure.Interfaces
{
    public interface ICache
    {
        void Put<T>(T instance);

        List<T> Get<T>();
    }
}
