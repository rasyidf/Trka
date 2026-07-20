using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects the release group. Uses these strategies in order:
    /// 1. Bracket group at the very beginning (anime style: [Group])
    /// 2. Last unmatched token that comes AFTER at least one matched property token (scene style: -GROUP)
    /// </summary>
    internal class ReleaseGroupMatcher : IMatcher
    {
        private static readonly Regex GroupPattern = new Regex(
            @"^[A-Za-z][A-Za-z0-9\-]{0,25}$", RegexOptions.Compiled);

        public void Match(Token[] tokens, GuessResult result)
        {
            // Strategy 1: first bracket group (anime-style [Group])
            foreach (var token in tokens)
            {
                if (token.IsBracketGroup && !token.Matched)
                {
                    if (GroupPattern.IsMatch(token.Value) && !IsKnownProperty(token.Value))
                    {
                        result.ReleaseGroup = token.Value;
                        token.Matched = true;
                        return;
                    }
                }
            }

            // Strategy 2: last unmatched non-bracket token AFTER at least one matched token
            // This ensures we don't steal title tokens from the beginning
            int lastMatchedIndex = -1;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Matched && !tokens[i].IsBracketGroup)
                    lastMatchedIndex = i;
            }

            if (lastMatchedIndex < 0) return; // no property matched, nothing to anchor on

            // Look for the last unmatched token after the last matched one
            for (int i = tokens.Length - 1; i > lastMatchedIndex; i--)
            {
                var token = tokens[i];
                if (token.Matched || token.IsBracketGroup) continue;
                if (GroupPattern.IsMatch(token.Value) && !IsKnownProperty(token.Value))
                {
                    result.ReleaseGroup = token.Value;
                    token.Matched = true;
                    return;
                }
                break; // only check the very last one
            }
        }

        private static bool IsKnownProperty(string value)
        {
            // ponytail: Quick reject list for common false positives
            var lower = value.ToLowerInvariant();
            return lower == "hdtv" || lower == "web" || lower == "dvd" || lower == "remux"
                || lower == "proper" || lower == "repack" || lower == "extended"
                || lower == "hevc" || lower == "avc";
        }
    }
}
