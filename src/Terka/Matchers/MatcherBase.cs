using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    /// <summary>
    /// Base interface for all property matchers.
    /// </summary>
    internal interface IMatcher
    {
        void Match(Token[] tokens, GuessResult result);
    }

    /// <summary>
    /// Matches a token value against a dictionary of patterns → canonical values.
    /// </summary>
    internal abstract class DictionaryMatcher : IMatcher
    {
        private readonly Dictionary<string, string> _map;
        private readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;

        protected DictionaryMatcher()
        {
            _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in BuildMap())
                _map[kv.Key] = kv.Value;
        }

        protected abstract IEnumerable<KeyValuePair<string, string>> BuildMap();
        protected abstract void Apply(GuessResult result, string canonicalValue);

        public void Match(Token[] tokens, GuessResult result)
        {
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                if (_map.TryGetValue(token.Value, out var canonical))
                {
                    Apply(result, canonical);
                    token.Matched = true;
                }
            }
        }
    }

    /// <summary>
    /// Matches a token value against regex patterns.
    /// </summary>
    internal abstract class RegexMatcher : IMatcher
    {
        private readonly List<(Regex Pattern, string Value)> _patterns;

        protected RegexMatcher()
        {
            _patterns = [];
            foreach (var entry in BuildPatterns())
                _patterns.Add((new Regex(entry.Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), entry.Value));
        }

        protected abstract IEnumerable<(string Pattern, string Value)> BuildPatterns();
        protected abstract void Apply(GuessResult result, string value, Match match);

        public void Match(Token[] tokens, GuessResult result)
        {
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                foreach (var (pattern, value) in _patterns)
                {
                    var m = pattern.Match(token.Value);
                    if (m.Success && m.Length == token.Value.Length)
                    {
                        Apply(result, value, m);
                        token.Matched = true;
                        break;
                    }
                }
            }
        }
    }
}
