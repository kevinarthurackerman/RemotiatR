using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoMapper.QuickMaps
{
    internal class TokenizedStringMatcher
    {
        private readonly Func<(string Value1, string Value2), bool> _matcher;

        internal TokenizedStringMatcher(string tokenizer1, string tokenizer2)
        {
            if (string.IsNullOrEmpty(tokenizer1)) throw new ArgumentException("Must not be null or empty", nameof(tokenizer1));
            if (string.IsNullOrEmpty(tokenizer2)) throw new ArgumentException("Must not be null or empty", nameof(tokenizer2));

            var tokenizer1Segments = GetSegments(tokenizer1, nameof(tokenizer1));
            var tokenizer1Regex = GetRegex(tokenizer1Segments);
            var tokenizer1Constants = GetConstantValues(tokenizer1Segments).ToArray();
            var tokenizer1TokenNames = GetTokens(tokenizer1Segments);
            var tokenizer1SegmentsCount = tokenizer1Segments.Count();

            var tokenizer2Segments = GetSegments(tokenizer2, nameof(tokenizer2));
            var tokenizer2Regex = GetRegex(tokenizer2Segments);
            var tokenizer2Constants = GetConstantValues(tokenizer2Segments).ToArray();
            var tokenizer2TokenNames = GetTokens(tokenizer2Segments);
            var tokenizer2SegmentsCount = tokenizer2Segments.Count();

            _matcher = ((string Value1, string Value2)x) =>
            {
                if (x.Value1 == x.Value2) return true;

                var value1Matches = tokenizer1Regex.Matches(x.Value1);

                if (!value1Matches.Any()) return false;

#if NETCOREAPP3_1
                var value1Segments = value1Matches.SelectMany(x => x.Groups.Values.Skip(1).Select(y => y.Value));
#endif
#if NETSTANDARD2_1
                var value1Segments = value1Matches.SelectMany(x => x.Groups.Skip(1).Select(y => y.Value));
#endif
                if (value1Segments.Count() != tokenizer1SegmentsCount) return false;

                var value1Constants = GetConstantValues(value1Segments).ToArray();
                for (var i = 0; i < value1Constants.Count(); i++)
                    if (value1Constants[i] != tokenizer1Constants[i]) return false;

                var value2Matches = tokenizer2Regex.Matches(x.Value2);

                if (!value2Matches.Any()) return false;


#if NETCOREAPP3_1
                var value2Segments = value2Matches.SelectMany(x => x.Groups.Values.Skip(1).Select(y => y.Value));
#endif
#if NETSTANDARD2_1
                var value2Segments = value2Matches.SelectMany(x => x.Groups.Skip(1).Select(y => y.Value));
#endif

                var value2Constants = GetConstantValues(value2Segments).ToArray();
                for (var i = 0; i < value2Constants.Count(); i++)
                    if (value2Constants[i] != tokenizer2Constants[i]) return false;

                var value1Tokens = tokenizer1TokenNames.Zip(GetTokens(value1Segments), (Name, Value) => (Name, Value));
                var value2Tokens = tokenizer2TokenNames.Zip(GetTokens(value2Segments), (Name, Value) => (Name, Value));

                foreach (var sourceToken in value1Tokens)
                    foreach (var destinationToken in value2Tokens)
                        if (sourceToken.Name == destinationToken.Name && sourceToken.Value != destinationToken.Value) return false;

                return true;
            };
        }

        public bool IsMatch(string value1, string value2) => _matcher((value1, value2));

        private static IEnumerable<string> GetSegments(string value, string paramName)
        {
            var segments = new List<string>();
            var segment = String.Empty;
            var inToken = false;
            char? previousCharacter = default;
            foreach (var character in value)
            {
                if (character == '{')
                    if (!inToken)
                        if (!previousCharacter.HasValue || previousCharacter.Value != '}')
                        {
                            segments.Add(segment);
                            segment = String.Empty;
                            inToken = true;
                        }
                        else throw new ArgumentException("Cannot follow a token with a token. Must be in format 'Some{value1}Thing{value2}", paramName);
                    else throw new ArgumentException("Tokens must be closed. Must be in format 'Some{value1}Thing{value2}", paramName);
                else if (character == '}')
                {
                    if (inToken)
                        if (!previousCharacter.HasValue || previousCharacter.Value != '{')
                        {
                            segments.Add(segment);
                            segment = String.Empty;
                            inToken = false;
                        }
                        else throw new ArgumentException("Tokens must have a name. Must be in format 'Some{value1}Thing{value2}", paramName);
                    else throw new ArgumentException("Tokens must be closed. Must be in format 'Some{value1}Thing{value2}", paramName);
                }
                else segment += character;
                previousCharacter = character;
            }
            if (inToken) throw new ArgumentException("Tokens must be closed. Must be in format 'Some{value1}Thing{value2}", paramName);

            if (segment != String.Empty) segments.Add(segment);

            return segments;
        }

        private static Regex GetRegex(IEnumerable<string> segments)
        {
            var regexBuilder = new StringBuilder("^");

            var isToken = false;
            foreach (var segment in segments)
            {
                if (isToken) regexBuilder.Append("(.+)");
                else if (segment != String.Empty)
                {
                    regexBuilder.Append("(");
                    regexBuilder.Append(Regex.Escape(segment));
                    regexBuilder.Append(")");
                }
                else regexBuilder.Append("()");
                isToken = !isToken;
            }

            regexBuilder.Append("$");

            return new Regex(regexBuilder.ToString());
        }

        private static IEnumerable<string> GetConstantValues(IEnumerable<string> segments)
        {
            var segmentsArray = segments.ToArray();
            var constantValues = new string[(int)Math.Ceiling(segmentsArray.Length / 2m)];
            var segmentsIndex = 0;
            for (var constantValuesIndex = 0; constantValuesIndex < constantValues.Length; constantValuesIndex++)
            {
                constantValues[constantValuesIndex] = segmentsArray[segmentsIndex];
                segmentsIndex += 2;
            }

            return constantValues;
        }

        private static IEnumerable<string> GetTokens(IEnumerable<string> segments)
        {
            var segmentsArray = segments.ToArray();
            var tokenNames = new string[segmentsArray.Length / 2];
            var segmentsIndex = 1;
            for (var tokenNamesIndex = 0; tokenNamesIndex < tokenNames.Length; tokenNamesIndex++)
            {
                tokenNames[tokenNamesIndex] = segmentsArray[segmentsIndex];
                segmentsIndex += 2;
            }

            return tokenNames;
        }
    }
}
