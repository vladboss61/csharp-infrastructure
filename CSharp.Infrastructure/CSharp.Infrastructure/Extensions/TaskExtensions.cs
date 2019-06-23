namespace CSharp.Infrastructure.Extensions
{
    using System;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static Task<TResult> Then<TInput, TResult>(this Task<TInput> self, Func<TInput, Task<TResult>> next)
        {
            return self.ContinueWith(x => next(x.Result)).Unwrap();
        }

        public static async Task Asynchronously(this Action work)
        {
            work.EnsureNotNull();
            await Task.Yield();
            work();
        }

        public static void Synchronously(this Action work)
        {
            work.EnsureNotNull();
            work();
        }

        public static Task<T> Asynchronously<T>(this Func<T> work)
        {
            work.EnsureNotNull();
            return Task.Run((() => work()));
        }
    }
}
