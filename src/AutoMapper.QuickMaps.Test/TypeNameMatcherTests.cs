using AutoMapper.QuickMaps.MappingMatchers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMapper.QuickMaps.Test
{
    [TestClass]
    public class TypeNameMatcherTests
    {
        [TestMethod]
        public void ShouldThrowArgumentExceptionOnInvalidMatcher()
        {
            var testValues = new[] { null, String.Empty, "{}", "{tokenA}{tokenB}", "{tokenA{tokenB}}Any", "Any{unclosedToken" };

            foreach(var testValue in testValues)
            {
                Assert.ThrowsException<ArgumentException>(() => DefaultMappingMatchers.TypeNameMatcher(testValue, "x"));
                Assert.ThrowsException<ArgumentException>(() => DefaultMappingMatchers.TypeNameMatcher("x", testValue));
            }
        }

        [TestMethod]
        public void ShouldMatchExact()
        {
            var expectedMatches = TestTypes.Select(x => (SourceType: x, DestinationType: x)).ToArray();

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{name}", "{name}", MappingMatcherBehaviors.MatchIdenticalType);

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceWithPrefix()
        {
            var expectedMatches = new[]
            {
                (typeof(PrefixClassA), typeof(ClassA)),
                (typeof(PrefixClassB), typeof(ClassB)),
                (typeof(PrefixClassASuffix), typeof(ClassASuffix)),
                (typeof(PrefixClassBSuffix), typeof(ClassBSuffix))
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("Prefix{name}", "{name}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceWithSuffix()
        {
            var expectedMatches = new[]
            {
                (typeof(ClassASuffix), typeof(ClassA)),
                (typeof(ClassBSuffix), typeof(ClassB)),
                (typeof(PrefixClassASuffix), typeof(PrefixClassA)),
                (typeof(PrefixClassBSuffix), typeof(PrefixClassB))
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{name}Suffix", "{name}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceWithPrefixAndSuffix()
        {
            var expectedMatches = new[]
            {
                (typeof(PrefixClassASuffix), typeof(ClassA)),
                (typeof(PrefixClassBSuffix), typeof(ClassB)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("Prefix{name}Suffix", "{name}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceWithInnerConstant()
        {
            var expectedMatches = new[]
            {
                (typeof(PrefixClassASuffix), typeof(ClassA)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{prefix}ClassA{suffix}", "ClassA");
            
            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchDestinationWithPrefix()
        {
            var expectedMatches = new[]
            {
                (typeof(ClassA), typeof(PrefixClassA)),
                (typeof(ClassB), typeof(PrefixClassB)),
                (typeof(ClassASuffix), typeof(PrefixClassASuffix)),
                (typeof(ClassBSuffix), typeof(PrefixClassBSuffix))
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{name}", "Prefix{name}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchDestinationWithSuffix()
        {
            var expectedMatches = new[]
            {
                (typeof(ClassA), typeof(ClassASuffix)),
                (typeof(ClassB), typeof(ClassBSuffix)),
                (typeof(PrefixClassA), typeof(PrefixClassASuffix)),
                (typeof(PrefixClassB), typeof(PrefixClassBSuffix))
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{name}", "{name}Suffix");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchDestinationWithPrefixAndSuffix()
        {
            var expectedMatches = new[]
            {
                (typeof(ClassA), typeof(PrefixClassASuffix)),
                (typeof(ClassB), typeof(PrefixClassBSuffix)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{name}", "Prefix{name}Suffix");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchDestinatonWithInnerConstant()
        {
            var expectedMatches = new[]
            {
                (typeof(ClassA), typeof(PrefixClassASuffix)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("ClassA", "{prefix}ClassA{suffix}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchTokensOutOfOrder()
        {
            var expectedMatches = new[]
            {
                (typeof(PrefixClassA), typeof(ClassASuffix)),
                (typeof(PrefixClassB), typeof(ClassBSuffix)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("Pre{fix}Class{x}", "Class{x}Suf{fix}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceParent()
        {
            var expectedMatches = new[]
            {
                (typeof(Parent.Child), typeof(Parent)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{parent}+{child}", "{parent}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchSourceChild()
        {
            var expectedMatches = new[]
            {
                (typeof(Parent), typeof(Parent.Child)),
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{parent}", "{parent}+{child}");

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        [TestMethod]
        public void ShouldMatchIgnoringCase()
        {
            var expectedMatches = new[]
            {
                (typeof(WordCase), typeof(WORDCASE)),
                (typeof(WordCase), typeof(wordcase)),
                (typeof(WORDCASE), typeof(WordCase)),
                (typeof(WORDCASE), typeof(wordcase)),
                (typeof(wordcase), typeof(WORDCASE)),
                (typeof(wordcase), typeof(WordCase))
            };

            var matcher = DefaultMappingMatchers.TypeNameMatcher("{token}", "{token}", MappingMatcherBehaviors.IgnoreCase);

            var actualMatches = TestTypePairs.Where(x => matcher(x.SourceType, x.DestinationType)).ToArray();

            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);
        }

        private static IEnumerable<Type> TestTypes { get; } = new[]
            {
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassASuffix),
                typeof(ClassBSuffix),
                typeof(PrefixClassA),
                typeof(PrefixClassB),
                typeof(PrefixClassASuffix),
                typeof(PrefixClassBSuffix),
                typeof(Prefix),
                typeof(Suffix),
                typeof(Parent),
                typeof(Parent.Child),
                typeof(WordCase),
                typeof(WORDCASE),
                typeof(wordcase)
            };

        private static IEnumerable<(Type SourceType, Type DestinationType)> SelfMatches { get; } = TestTypes.Select(x => (x, x)).ToArray();

        private static IEnumerable<(Type SourceType, Type DestinationType)> TestTypePairs { get; } = TestTypes.SelectMany(x => TestTypes.Select(y => (x, y))).ToArray();
    }

    public class ClassA { }

    public class ClassB { }

    public class ClassASuffix { }

    public class ClassBSuffix { }

    public class PrefixClassA { }

    public class PrefixClassB { }

    public class PrefixClassASuffix { }

    public class PrefixClassBSuffix { }

    public class Prefix { }

    public class Suffix { }

    public class Parent
    {
        public class Child { }
    }

    public class WordCase { }

    public class WORDCASE { }

    public class wordcase { }
}
