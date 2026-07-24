using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    internal class SourceMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.Sources)
                yield return entry;

            // Multi-word / separator entries (base tokenizer regex-matches these)
            yield return new KeyValuePair<string, string>("blu-ray", "Blu-ray");
            yield return new KeyValuePair<string, string>("web-dl", "Web");
            yield return new KeyValuePair<string, string>("web-rip", "Web");
            yield return new KeyValuePair<string, string>("dvd-r", "DVD");
            yield return new KeyValuePair<string, string>("hd-dvd", "HD-DVD");
            yield return new KeyValuePair<string, string>("uhd.bluray", "Ultra HD Blu-ray");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.Source = canonicalValue;
        }
    }
}
