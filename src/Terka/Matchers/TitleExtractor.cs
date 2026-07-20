using System.Collections.Generic;
using System.Text;

namespace Terka.Matchers
{
    /// <summary>
    /// Extracts the title from the leading unmatched tokens.
    /// Title is everything before the first matched property token.
    /// </summary>
    internal class TitleExtractor
    {
        public void Extract(Token[] tokens, GuessResult result)
        {
            var titleParts = new List<string>();

            // Skip leading bracket groups (often release group in anime)
            int start = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].IsBracketGroup) continue;
                start = i;
                break;
            }

            // Collect consecutive unmatched non-bracket tokens from start
            for (int i = start; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.IsBracketGroup) continue;
                if (token.Matched) break;
                titleParts.Add(token.Value);
                token.Matched = true;
            }

            if (titleParts.Count > 0)
            {
                result.Title = string.Join(" ", titleParts);
            }
        }
    }
}
