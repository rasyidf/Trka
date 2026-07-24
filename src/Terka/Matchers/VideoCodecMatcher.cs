using System.Collections.Generic;
using Terka.Shared;

namespace Terka.Matchers
{
    internal class VideoCodecMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // Primary data from shared vocabulary
            foreach (var entry in Vocabulary.VideoCodecs)
                yield return entry;

            // Separator entries (base tokenizer regex-matches these)
            yield return new KeyValuePair<string, string>("h.264", "H.264");
            yield return new KeyValuePair<string, string>("h.265", "H.265");
            yield return new KeyValuePair<string, string>("mpeg-2", "MPEG-2");
            yield return new KeyValuePair<string, string>("vc-1", "VC-1");
            yield return new KeyValuePair<string, string>("h.263", "H.263");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.VideoCodec = canonicalValue;
        }
    }
}
