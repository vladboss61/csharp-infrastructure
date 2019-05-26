namespace CSharp.Infrastructure.Interfaces
{
    public interface IPool<T> where T : class
    {
        Guard<T> Rent();
        
        void Return(T item);
    }
}
