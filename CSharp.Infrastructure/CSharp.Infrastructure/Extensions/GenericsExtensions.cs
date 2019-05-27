using System;

namespace CSharp.Infrastructure.Extensions
{
    public static class GenericsExtensions
    {
        public static TNew As<TOld, TNew>(this TOld self, Func<TOld, TNew> map) =>
            map(self);

        public static T With<T>(this T self, Action<T> set) where T : class
        {
            set(self);
            return self;
        }

        public static TResult Using<T, TResult>(this T self, Func<T, TResult> map) where T : class, IDisposable
        {
            using (self)
            {
                return map(self);
            }
        }

        public static T Do<T>(this T self, Action<T> move)
        {
            move(self);
            return self;
        }

        public static T Or<T>(this T self, Func<T> otherwise) =>
            self != null ? self : otherwise();

        public static T Or<T>(this T self, T otherwise) =>
            self != null ? self : otherwise;
    }
}
