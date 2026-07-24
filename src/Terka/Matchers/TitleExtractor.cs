using System.Collections.Generic;

namespace Terka.Matchers
{
    /// <summary>
    /// Extracts the title from the leading unmatched tokens.
    /// Also extracts episode title (unmatched tokens after the episode marker).
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

            // Episode title: unmatched tokens immediately after the episode marker
            if (result.Season.Count > 0 || result.Episode.Count > 0)
            {
                ExtractEpisodeTitle(tokens, result);
            }
        }

        private static void ExtractEpisodeTitle(Token[] tokens, GuessResult result)
        {
            // Find the first matched token that looks like S##E## or NxNN (episode marker)
            int epIndex = -1;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (!tokens[i].Matched || tokens[i].IsBracketGroup) continue;
                var val = tokens[i].Value;
                if (val.Length >= 4 &&
                    (val[0] == 'S' || val[0] == 's') &&
                    char.IsDigit(val[1]))
                {
                    epIndex = i;
                    break;
                }
                // NxNN
                int xPos = val.IndexOf('x');
                if (xPos < 0) xPos = val.IndexOf('X');
                if (xPos > 0 && xPos < val.Length - 1 && char.IsDigit(val[0]))
                {
                    epIndex = i;
                    break;
                }
            }

            if (epIndex < 0) return;

            // Collect consecutive unmatched non-bracket tokens after episode marker
            var epTitleParts = new List<string>();
            int epTitleStart = epIndex + 1;
            while (epTitleStart < tokens.Length && (tokens[epTitleStart].Matched || tokens[epTitleStart].IsBracketGroup))
                epTitleStart++;

            for (int i = epTitleStart; i < tokens.Length; i++)
            {
                if (tokens[i].Matched || tokens[i].IsBracketGroup) break;
                epTitleParts.Add(tokens[i].Value);
                tokens[i].Matched = true;
            }

            if (epTitleParts.Count > 0)
            {
                result.EpisodeTitle = string.Join(" ", epTitleParts);
            }
        }
    }
}
