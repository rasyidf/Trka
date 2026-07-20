using System.Collections.Generic;

namespace Terka.Matchers
{
    internal class ColorDepthMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            yield return new KeyValuePair<string, string>("8bit", "8-bit");
            yield return new KeyValuePair<string, string>("8-bit", "8-bit");
            yield return new KeyValuePair<string, string>("10bit", "10-bit");
            yield return new KeyValuePair<string, string>("10-bit", "10-bit");
            yield return new KeyValuePair<string, string>("12bit", "12-bit");
            yield return new KeyValuePair<string, string>("12-bit", "12-bit");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.ColorDepth = canonicalValue;
        }
    }
}
