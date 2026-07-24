using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    internal class ColorDepthMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.ColorDepths)
                yield return entry;

            // Additional separator variants (base tokenizer can handle these)
            yield return new KeyValuePair<string, string>("8-bit", "8-bit");
            yield return new KeyValuePair<string, string>("10-bit", "10-bit");
            yield return new KeyValuePair<string, string>("12-bit", "12-bit");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.ColorDepth = canonicalValue;
        }
    }
}
