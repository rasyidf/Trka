using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    internal class ScreenSizeMatcher : RegexMatcher
    {
        protected override IEnumerable<(string Pattern, string Value)> BuildPatterns()
        {
            yield return (@"4320[pi]", null); // use matched value
            yield return (@"2160[pi]", null);
            yield return (@"1440[pi]", null);
            yield return (@"1080[pi]", null);
            yield return (@"900[pi]", null);
            yield return (@"720[pi]", null);
            yield return (@"576[pi]", null);
            yield return (@"540[pi]", null);
            yield return (@"480[pi]", null);
            yield return (@"360[pi]", null);
            yield return (@"\d{3,4}x\d{3,4}", null); // e.g. 1920x1080
        }

        protected override void Apply(GuessResult result, string value, Match match)
        {
            result.ScreenSize = match.Value.ToLowerInvariant();
        }
    }
}
