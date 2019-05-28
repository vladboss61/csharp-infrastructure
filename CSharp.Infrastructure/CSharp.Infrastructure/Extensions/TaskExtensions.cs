using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSharp.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        public static Task<TResult> Then<TInput, TResult>(this Task<TInput> self, Func<TInput, Task<TResult>> next)
        {
            return self.ContinueWith(x => next(x.Result)).Unwrap();
        }

        public static async Task Asynchronously(this Action move)
        {
            await Task.Yield();
            move();
        }
    }
}
