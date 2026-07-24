using System.Text.RegularExpressions;
using Terka.Shared;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects a 4-digit year in tokens using shared year bounds.
    /// </summary>
    internal class YearMatcher : IMatcher
    {
        private static readonly Regex YearPattern = new Regex(
            @"^((?:19|20)\d{2})$", RegexOptions.Compiled);

        public void Match(Token[] tokens, GuessResult result)
        {
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                var m = YearPattern.Match(token.Value);
                if (m.Success)
                {
                    int year = int.Parse(m.Groups[1].Value);
                    if (year >= Vocabulary.YearMin && year <= Vocabulary.YearMax)
                    {
                        result.Year = year;
                        token.Matched = true;
                        return;
                    }
                }
            }
        }
    }
}
