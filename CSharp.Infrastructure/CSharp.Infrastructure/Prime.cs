using System;
using System.Collections.Generic;

namespace CSharp.Infrastructure
{
    public class Prime
    {
        private readonly long _number;

        public Prime(int number)
        {
            EnsureSize(number);
            _number = number;
        }

        public Prime(long number)
        {
            EnsureSize(number);
            _number = number;
        }

        public IEnumerable<int> SieveEratosthenes()
        {
            var composite = new bool[_number + 1];
            for (var p = 2; p <= _number; p++)
            {
                if (composite[p])
                {
                    continue;
                }
                yield return p;

                for (var i = p * p; i <= _number; i += p)
                {
                    composite[i] = true;
                }
            }
        }

        public static Prime Create(int number)
        {
            return new Prime(number);
        }

        private static void EnsureSize(long number)
        {
            if (number <= 0)
            {
                throw new InvalidOperationException($"The {number} have inconvenient value.");
            }
        }
    }
}
