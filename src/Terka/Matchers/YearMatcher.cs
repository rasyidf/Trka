using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects a 4-digit year (1920–2030) in tokens.
    /// </summary>
    internal class YearMatcher : IMatcher
    {
        private static readonly Regex YearPattern = new Regex(
            @"^((?:19|20)\d{2})$", RegexOptions.Compiled);

        public void Match(Token[] tokens, GuessResult result)
        {
            // Take the first plausible year token
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                var m = YearPattern.Match(token.Value);
                if (m.Success)
                {
                    int year = int.Parse(m.Groups[1].Value);
                    if (year >= 1920 && year <= 2030)
                    {
                        result.Year = year;
                        token.Matched = true;
                        return; // only one year
                    }
                }
            }
        }
    }
}
