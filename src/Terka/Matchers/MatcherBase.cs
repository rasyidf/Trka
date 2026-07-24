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
    /// Supports two-token lookahead for multi-word patterns split by tokenizer.
    /// </summary>
    internal abstract class DictionaryMatcher : IMatcher
    {
        private readonly Dictionary<string, string> _map;
        private readonly Dictionary<string, string> _twoTokenMap;

        protected DictionaryMatcher()
        {
            _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _twoTokenMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in BuildMap())
            {
                // Entries with spaces are two-token patterns: split and store as "first\0second"
                if (kv.Key.IndexOf(' ') >= 0)
                {
                    var parts = kv.Key.Split([' '], 2);
                    _twoTokenMap[parts[0] + "\0" + parts[1]] = kv.Value;
                }
                else
                {
                    _map[kv.Key] = kv.Value;
                }
            }
        }

        protected abstract IEnumerable<KeyValuePair<string, string>> BuildMap();
        protected abstract void Apply(GuessResult result, string canonicalValue);

        public void Match(Token[] tokens, GuessResult result)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.Matched) continue;

                // Two-token lookahead
                if (_twoTokenMap.Count > 0 && i + 1 < tokens.Length)
                {
                    int next = i + 1;
                    while (next < tokens.Length && tokens[next].Matched) next++;
                    if (next < tokens.Length)
                    {
                        var key = token.Value + "\0" + tokens[next].Value;
                        if (_twoTokenMap.TryGetValue(key, out var twoCanonical))
                        {
                            Apply(result, twoCanonical);
                            token.Matched = true;
                            tokens[next].Matched = true;
                            continue;
                        }
                    }
                }

                // Single-token match
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
