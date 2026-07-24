using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    internal class EditionMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.Editions)
                yield return entry;

            // Multi-word entries (base tokenizer regex/multi-token matching)
            yield return new KeyValuePair<string, string>("directors cut", "Director's Cut");
            yield return new KeyValuePair<string, string>("director's cut", "Director's Cut");
            yield return new KeyValuePair<string, string>("extended cut", "Extended");
            yield return new KeyValuePair<string, string>("theatrical cut", "Theatrical");
            yield return new KeyValuePair<string, string>("special edition", "Special");
            yield return new KeyValuePair<string, string>("collector's edition", "Collector");
            yield return new KeyValuePair<string, string>("fan edit", "Fan");
            yield return new KeyValuePair<string, string>("alternative cut", "Alternative Cut");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            if (!result.Edition.Contains(canonicalValue))
                result.Edition.Add(canonicalValue);
        }
    }

    internal class OtherMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.Others)
                yield return entry;

            // Multi-word / separator entries
            yield return new KeyValuePair<string, string>("hdr10+", "HDR10");
            yield return new KeyValuePair<string, string>("dolby vision", "Dolby Vision");
            yield return new KeyValuePair<string, string>("dual audio", "Dual Audio");
            yield return new KeyValuePair<string, string>("open matte", "Open Matte");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            if (!result.Other.Contains(canonicalValue))
                result.Other.Add(canonicalValue);
        }
    }
}
