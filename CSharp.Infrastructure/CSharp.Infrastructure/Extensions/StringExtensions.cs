using System.Collections.Generic;
using System.Linq;

namespace CSharp.Infrastructure.Extensions
{
    public static class StringExtensions
    {
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
    }
}
