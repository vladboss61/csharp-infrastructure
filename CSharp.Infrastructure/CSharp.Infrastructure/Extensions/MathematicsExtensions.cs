using System;

namespace CSharp.Infrastructure.Extensions
{
    public static class MathematicsExtensions
    {
        public static bool IsNearlyZero(this double number) =>
            Math.Abs(number) <= double.Epsilon;

        public static bool IsNearlyZero(this float number) =>
            Math.Abs(number) <= float.Epsilon;
    }
}
