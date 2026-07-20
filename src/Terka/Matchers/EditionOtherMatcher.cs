using System.Collections.Generic;

namespace Terka.Matchers
{
    internal class EditionMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            yield return new KeyValuePair<string, string>("dc", "Director's Cut");
            yield return new KeyValuePair<string, string>("directors cut", "Director's Cut");
            yield return new KeyValuePair<string, string>("director's cut", "Director's Cut");
            yield return new KeyValuePair<string, string>("extended", "Extended");
            yield return new KeyValuePair<string, string>("extended cut", "Extended");
            yield return new KeyValuePair<string, string>("unrated", "Unrated");
            yield return new KeyValuePair<string, string>("uncut", "Uncut");
            yield return new KeyValuePair<string, string>("remastered", "Remastered");
            yield return new KeyValuePair<string, string>("restored", "Restored");
            yield return new KeyValuePair<string, string>("theatrical", "Theatrical");
            yield return new KeyValuePair<string, string>("theatrical cut", "Theatrical");
            yield return new KeyValuePair<string, string>("imax", "IMAX");
            yield return new KeyValuePair<string, string>("special edition", "Special");
            yield return new KeyValuePair<string, string>("special", "Special");
            yield return new KeyValuePair<string, string>("limited", "Limited");
            yield return new KeyValuePair<string, string>("collector", "Collector");
            yield return new KeyValuePair<string, string>("collector's edition", "Collector");
            yield return new KeyValuePair<string, string>("criterion", "Criterion");
            yield return new KeyValuePair<string, string>("ultimate", "Ultimate");
            yield return new KeyValuePair<string, string>("deluxe", "Deluxe");
            yield return new KeyValuePair<string, string>("uncensored", "Uncensored");
            yield return new KeyValuePair<string, string>("fan", "Fan");
            yield return new KeyValuePair<string, string>("fan edit", "Fan");
            yield return new KeyValuePair<string, string>("alternative cut", "Alternative Cut");
            yield return new KeyValuePair<string, string>("festival", "Festival");
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
            yield return new KeyValuePair<string, string>("proper", "Proper");
            yield return new KeyValuePair<string, string>("repack", "Proper");
            yield return new KeyValuePair<string, string>("rerip", "Proper");
            yield return new KeyValuePair<string, string>("remux", "Remux");
            yield return new KeyValuePair<string, string>("3d", "3D");
            yield return new KeyValuePair<string, string>("hdr", "HDR10");
            yield return new KeyValuePair<string, string>("hdr10", "HDR10");
            yield return new KeyValuePair<string, string>("hdr10+", "HDR10");
            yield return new KeyValuePair<string, string>("dv", "Dolby Vision");
            yield return new KeyValuePair<string, string>("dolby vision", "Dolby Vision");
            yield return new KeyValuePair<string, string>("dolbyvision", "Dolby Vision");
            yield return new KeyValuePair<string, string>("dual", "Dual Audio");
            yield return new KeyValuePair<string, string>("dual audio", "Dual Audio");
            yield return new KeyValuePair<string, string>("multi", "Dual Audio");
            yield return new KeyValuePair<string, string>("complete", "Complete");
            yield return new KeyValuePair<string, string>("internal", "Internal");
            yield return new KeyValuePair<string, string>("sample", "Sample");
            yield return new KeyValuePair<string, string>("fix", "Fix");
            yield return new KeyValuePair<string, string>("hardcoded", "Hardcoded Subtitles");
            yield return new KeyValuePair<string, string>("hc", "Hardcoded Subtitles");
            yield return new KeyValuePair<string, string>("subbed", "Hardcoded Subtitles");
            yield return new KeyValuePair<string, string>("dubbed", "Line Dubbed");
            yield return new KeyValuePair<string, string>("dub", "Line Dubbed");
            yield return new KeyValuePair<string, string>("screener", "Screener");
            yield return new KeyValuePair<string, string>("scr", "Screener");
            yield return new KeyValuePair<string, string>("trailer", "Trailer");
            yield return new KeyValuePair<string, string>("documentary", "Documentary");
            yield return new KeyValuePair<string, string>("doc", "Documentary");
            yield return new KeyValuePair<string, string>("xxx", "XXX");
            yield return new KeyValuePair<string, string>("hybrid", "Hybrid");
            yield return new KeyValuePair<string, string>("open matte", "Open Matte");
            yield return new KeyValuePair<string, string>("uhd", "Ultra HD");
            yield return new KeyValuePair<string, string>("fullhd", "Full HD");
            yield return new KeyValuePair<string, string>("hd", "HD");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            if (!result.Other.Contains(canonicalValue))
                result.Other.Add(canonicalValue);
        }
    }
}
