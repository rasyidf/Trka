using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    internal class StreamingServiceMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.StreamingServices)
                yield return entry;

            // Entries with special chars that only base tokenizer handles
            yield return new KeyValuePair<string, string>("disney+", "Disney+");
            yield return new KeyValuePair<string, string>("appletv+", "AppleTV");
            yield return new KeyValuePair<string, string>("paramount+", "Paramount+");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.StreamingService = canonicalValue;
        }
    }
}
