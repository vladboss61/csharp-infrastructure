using System;
using System.Reflection;

namespace CSharp.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        public static Type DirectParent(this Type self)
        {
            self.EnsureNotNull();

            var typeInfo = self.GetTypeInfo();
            return typeInfo.BaseType;
        }

        public static bool IsAlso<TParent>(this Type self)
        {
            self.EnsureNotNull();

            var typeInfo = self.GetTypeInfo();
            return typeof(TParent).IsAssignableFrom(typeInfo);
        }

        public static bool InHierarchyWith<TParent>(this Type self)
        {
            return self.IsAlso<TParent>();
        }

        public static bool IsConcrete(this Type self)
        {
            if (self == null)
            {
                return false;
            }

            var typeInfo = self.GetTypeInfo();

            return !(typeInfo.IsAbstract || typeInfo.IsInterface);
        }

        public static bool IsNullable(this Type self)
        {
            self.EnsureNotNull();

            var typeInfo = self.GetTypeInfo();
            return typeInfo.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool Has<T, TAttribute>(this T self) 
            where TAttribute : Attribute 
            where T: class
        {
            self.EnsureNotNull();

            return Attribute.IsDefined(self.GetType(), typeof(TAttribute));
        }
    }
}
