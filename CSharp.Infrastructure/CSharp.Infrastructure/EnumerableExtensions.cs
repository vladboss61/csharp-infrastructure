using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> self) =>
            self ?? Enumerable.Empty<T>();

        public static IEnumerable<string> AlternativeCase(this IEnumerable<string> self) =>
            self.Select(Alternative);

        public static string Alternative(string word) =>
            word.Select((letter, position) => position % 2 == 0 ? char.ToUpper(letter) : char.ToLower(letter))
                .Join(with: string.Empty);

        public static string Join(this IEnumerable<string> self, char with) =>
            string.Join(with.ToString(), self);

        public static IEnumerable<string> Split(this string self, char by) =>
            self.Split(by);

        public static string Join(this IEnumerable<char> self, string with) =>
            string.Join(with, self);

        public static bool IsNullOrWhiteSpace(this string self) =>
            string.IsNullOrWhiteSpace(self);

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var value in self)
            {
                action(value);
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T appended)
        {
            self.EnsureArgs();

            foreach (var element in self)
            {
                yield return element;
            }

            yield return appended;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, T prepended)
        {
            self.EnsureArgs();

            yield return prepended;

            foreach (var element in self)
            {
                yield return element;
            }
        }

        public static bool EndsWith<T>(this IEnumerable<T> self, IEnumerable<T> suffix)
        {
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

        private static void EnsureArgs<T>(this T self) where T : class
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
        }
    }
}
