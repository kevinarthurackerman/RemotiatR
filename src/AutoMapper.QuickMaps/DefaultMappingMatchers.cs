using System;

namespace AutoMapper.QuickMaps
{
    public static class DefaultMappingMatchers
    {
        public static MappingMatcher TypeNameMatcher(string sourceTypeNameMatcher, string destinationTypeNameMatcher, MappingMatcherBehaviors matcherBehaviors = MappingMatcherBehaviors.None)
        {
            if (string.IsNullOrEmpty(sourceTypeNameMatcher)) throw new ArgumentException("Must not be null or empty", nameof(sourceTypeNameMatcher));
            if (string.IsNullOrEmpty(destinationTypeNameMatcher)) throw new ArgumentException("Must not be null or empty", nameof(destinationTypeNameMatcher));

            var tokenizedStringMatcher = new TokenizedStringMatcher(sourceTypeNameMatcher, destinationTypeNameMatcher);

            return (Type sourceType, Type destinationType) =>
            {
                if (sourceType == destinationType) return matcherBehaviors.HasFlag(MappingMatcherBehaviors.MatchIdenticalType);

                var sourceTypeName = sourceType.ExtendedName();
                var destinationTypeName = destinationType.ExtendedName();

                if (matcherBehaviors.HasFlag(MappingMatcherBehaviors.IgnoreCase))
                {
                    sourceTypeName = sourceTypeName.ToLower();
                    destinationTypeName = destinationTypeName.ToLower();
                }

                if (matcherBehaviors.HasFlag(MappingMatcherBehaviors.MatchIdenticalName) && sourceTypeName == destinationTypeName) return true;

                return tokenizedStringMatcher.IsMatch(sourceTypeName, destinationTypeName);
            };
        }

        public static MappingMatcher ExactMatcher() => PrefixSuffixMatcher(String.Empty, String.Empty, String.Empty, String.Empty);

        public static MappingMatcher PrefixMatcher(string prefix) => PrefixSuffixMatcher(prefix, String.Empty, prefix, String.Empty);

        public static MappingMatcher PrefixMatcher(string sourcePrefix, string destinationPrefix) => PrefixSuffixMatcher(sourcePrefix, String.Empty, destinationPrefix, String.Empty);

        public static MappingMatcher SuffixMatcher(string suffix) => PrefixSuffixMatcher(String.Empty, suffix, String.Empty, suffix);

        public static MappingMatcher SuffixMatcher(string sourceSuffix, string destinationSuffix) => PrefixSuffixMatcher(String.Empty, sourceSuffix, String.Empty, destinationSuffix);

        public static MappingMatcher SourcePrefixMatcher(string prefix) => PrefixSuffixMatcher(prefix, String.Empty, String.Empty, String.Empty);

        public static MappingMatcher SourceSuffixMatcher(string suffix) => PrefixSuffixMatcher(String.Empty, suffix, String.Empty, String.Empty);

        public static MappingMatcher DestinationPrefixMatcher(string prefix) => PrefixSuffixMatcher(String.Empty, String.Empty, prefix, String.Empty);

        public static MappingMatcher DestinationSuffixMatcher(string suffix) => PrefixSuffixMatcher(String.Empty, String.Empty, String.Empty, suffix);

        public static MappingMatcher PrefixSuffixMatcher(string prefix, string suffix) => PrefixSuffixMatcher(prefix, suffix, prefix, suffix);

        public static MappingMatcher PrefixSuffixMatcher(string sourcePrefix, string sourceSuffix, string destinationPrefix, string destinationSuffix, MappingMatcherBehaviors matcherBehaviors = MappingMatcherBehaviors.None)
        {
            if (string.IsNullOrEmpty(sourcePrefix)) throw new ArgumentException("Must not be null or empty", nameof(sourcePrefix));
            if (string.IsNullOrEmpty(sourceSuffix)) throw new ArgumentException("Must not be null or empty", nameof(sourceSuffix));
            if (string.IsNullOrEmpty(destinationPrefix)) throw new ArgumentException("Must not be null or empty", nameof(destinationPrefix));
            if (string.IsNullOrEmpty(destinationSuffix)) throw new ArgumentException("Must not be null or empty", nameof(destinationSuffix));

            return (Type sourceType, Type destinationType) =>
            {
                if (sourceType == destinationType) return matcherBehaviors.HasFlag(MappingMatcherBehaviors.MatchIdenticalType);

                var sourceTypeName = sourceType.ExtendedName();
                var destinationTypeName = destinationType.ExtendedName();

                if (matcherBehaviors.HasFlag(MappingMatcherBehaviors.IgnoreCase))
                {
                    sourceTypeName = sourceTypeName.ToLower();
                    destinationTypeName = destinationTypeName.ToLower();
                }

                if (matcherBehaviors.HasFlag(MappingMatcherBehaviors.MatchIdenticalName) && sourceTypeName == destinationTypeName) return true;

                if (sourceTypeName.Length <= sourcePrefix.Length + sourceSuffix.Length) return false;
                if (destinationTypeName.Length <= destinationPrefix.Length + destinationSuffix.Length) return false;

                sourceTypeName = sourceTypeName.Substring(sourcePrefix.Length, sourceTypeName.Length - sourcePrefix.Length - sourceSuffix.Length);
                destinationTypeName = destinationTypeName.Substring(destinationPrefix.Length, destinationTypeName.Length - destinationPrefix.Length - destinationSuffix.Length);

                return sourceTypeName == destinationTypeName;
            };
        }
    }
}
