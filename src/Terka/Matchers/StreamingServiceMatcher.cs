using System.Collections.Generic;

namespace Terka.Matchers
{
    internal class StreamingServiceMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            yield return new KeyValuePair<string, string>("amzn", "Amazon Prime");
            yield return new KeyValuePair<string, string>("amazon", "Amazon Prime");
            yield return new KeyValuePair<string, string>("nf", "Netflix");
            yield return new KeyValuePair<string, string>("netflix", "Netflix");
            yield return new KeyValuePair<string, string>("dsnp", "Disney+");
            yield return new KeyValuePair<string, string>("disney+", "Disney+");
            yield return new KeyValuePair<string, string>("disneyplus", "Disney+");
            yield return new KeyValuePair<string, string>("hulu", "Hulu");
            yield return new KeyValuePair<string, string>("hmax", "HBO Max");
            yield return new KeyValuePair<string, string>("hbo", "HBO Max");
            yield return new KeyValuePair<string, string>("atvp", "AppleTV");
            yield return new KeyValuePair<string, string>("aptv", "AppleTV");
            yield return new KeyValuePair<string, string>("appletv+", "AppleTV");
            yield return new KeyValuePair<string, string>("pcok", "Peacock");
            yield return new KeyValuePair<string, string>("peacock", "Peacock");
            yield return new KeyValuePair<string, string>("pmtp", "Paramount+");
            yield return new KeyValuePair<string, string>("paramount+", "Paramount+");
            yield return new KeyValuePair<string, string>("cr", "Crunchy Roll");
            yield return new KeyValuePair<string, string>("crunchyroll", "Crunchy Roll");
            yield return new KeyValuePair<string, string>("stan", "Stan");
            yield return new KeyValuePair<string, string>("crav", "Crave");
            yield return new KeyValuePair<string, string>("crave", "Crave");
            yield return new KeyValuePair<string, string>("it", "iTunes");
            yield return new KeyValuePair<string, string>("itunes", "iTunes");
            yield return new KeyValuePair<string, string>("red", "YouTube Red");
            yield return new KeyValuePair<string, string>("yt", "YouTube Red");
            yield return new KeyValuePair<string, string>("sho", "Showtime");
            yield return new KeyValuePair<string, string>("showtime", "Showtime");
            yield return new KeyValuePair<string, string>("starz", "Starz");
            yield return new KeyValuePair<string, string>("max", "Max");
            yield return new KeyValuePair<string, string>("iqiyi", "iQIYI");
            yield return new KeyValuePair<string, string>("binge", "Binge");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.StreamingService = canonicalValue;
        }
    }
}
