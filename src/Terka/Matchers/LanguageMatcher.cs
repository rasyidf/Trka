using System;
using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects language from tokens using the shared vocabulary.
    /// </summary>
    internal class LanguageMatcher : IMatcher
    {
        private static readonly Dictionary<string, string> Map = BuildMap();

        private static Dictionary<string, string> BuildMap()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in Vocabulary.Languages)
                map[entry.Key] = entry.Value;
            return map;
        }

        public void Match(Token[] tokens, GuessResult result)
        {
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                if (Map.TryGetValue(token.Value, out var language))
                {
                    result.Language = language;
                    token.Matched = true;
                    return;
                }
            }
        }
    }
}
