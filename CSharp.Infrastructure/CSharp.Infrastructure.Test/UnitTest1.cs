using System.Collections.Generic;
using CSharp.Infrastructure.Extensions;
using CSharp.Infrastructure.Interfaces;
using Xunit;

namespace CSharp.Infrastructure.Test
{
    public sealed class UnitTest1
    {
        [Fact]
        public void TestExtensionStringsSubstring()
        {
            var resultActual1 = "string_short".Substring(1, 4, Boundary.IncludeBoth);
            Assert.Equal("trin", resultActual1);

            var resultActual2 = "string_short".Substring(1, 4, Boundary.IncludeLeft);
            Assert.Equal("tri", resultActual2);

            var resultActual3 = "string_short".Substring(1, 4, Boundary.IncludeRight);
            Assert.Equal("rin", resultActual3);
        }

        [Fact]
        public void TestExtensionOfType()
        {
            var result = typeof(List<>).DirectParent();
            Assert.True(result == typeof(object));
        }

        [Fact]
        public void TestExtensionOfIsAlso()
        {
            Assert.True(typeof(MemorySettings).IsAlso<ISettings>());
            Assert.True(typeof(MemorySettings).InHierarchyWith<object>());
            Assert.True(typeof(A).IsAlso<object>());
            Assert.True(typeof(B).IsAlso<A>());
            Assert.True(typeof(C).IsAlso<B>());
            Assert.True(typeof(C).IsAlso<A>());
        }


        [Fact]
        public void TestExtensionOfType_Should_Be_B_Class()
        {
            var result = typeof(C).DirectParent();
            Assert.True(result == typeof(B));
            Assert.True(result.DirectParent() == typeof(A));
            Assert.True(result.DirectParent().DirectParent() == typeof(object));
            Assert.True(result.DirectParent().DirectParent().DirectParent() == null);
        }

        class A
        {

        }

        class B : A
        {

        }

        class C : B
        {

        }
    }
}
