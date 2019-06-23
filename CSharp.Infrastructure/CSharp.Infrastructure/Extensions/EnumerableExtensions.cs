using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> self) => 
            !self.Any();

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> self) =>
            self ?? Enumerable.Empty<T>();

        public static IEnumerable<Tuple<int, T>> Index<T>(this IEnumerable<T> self) =>
            self.Select((item, index) => new Tuple<int, T>(index, item));

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            self.EnsureNotNull();
            action.EnsureNotNull();

            foreach (var value in self)
            {
                action(value);
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T appended) where T : class
        {
            self.EnsureNotNull();
            appended.EnsureNotNull();

            foreach (var element in self)
            {
                yield return element;
            }

            yield return appended;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, T prepended) where T : class
        {
            self.EnsureNotNull();
            prepended.EnsureNotNull();

            yield return prepended;

            foreach (var element in self)
            {
                yield return element;
            }
        }

        public static bool EndsWith<T>(this IEnumerable<T> self, IEnumerable<T> suffix)
        {
            self.EnsureNotNull();
            suffix.EnsureNotNull();

            var leftCount = self.Count();
            var rightCount = suffix.Count();

            if (leftCount < rightCount)
            {
                return false;
            }

            var shift = leftCount - rightCount;

            using (var selfIterator = self.GetEnumerator())
            {
                foreach (var _ in Enumerable.Range(0, shift))
                {
                    selfIterator.MoveNext();
                }

                using (var suffixIterator = suffix.GetEnumerator())
                {
                    while (selfIterator.MoveNext() && suffixIterator.MoveNext())
                    {
                        if (!selfIterator.Current.Equals(suffixIterator.Current))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public static bool StartsWith<T>(this IEnumerable<T> self, IEnumerable<T> prefix)
        {
            self.EnsureNotNull();
            prefix.EnsureNotNull();

            using (var selfIterator = self.GetEnumerator())
            using (var prefixIterator = prefix.GetEnumerator())
            {

                while (selfIterator.MoveNext() && prefixIterator.MoveNext())
                {
                    if (selfIterator.Current != null && !selfIterator.Current.Equals(prefixIterator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> self, Func<TSource, bool> predicate)
        {
            return self.TakeWhile(item => !predicate(item)).Count();
        }

        public static void EnsureNotNull<T>(this T self) where T : class
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
        }
    }
}
