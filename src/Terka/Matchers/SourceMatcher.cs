using System.Collections.Generic;

namespace Terka.Matchers
{
    internal class SourceMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            yield return new KeyValuePair<string, string>("bluray", "Blu-ray");
            yield return new KeyValuePair<string, string>("blu-ray", "Blu-ray");
            yield return new KeyValuePair<string, string>("bdrip", "Blu-ray");
            yield return new KeyValuePair<string, string>("brrip", "Blu-ray");
            yield return new KeyValuePair<string, string>("bdremux", "Blu-ray");

            yield return new KeyValuePair<string, string>("uhd.bluray", "Ultra HD Blu-ray");
            yield return new KeyValuePair<string, string>("uhdbluray", "Ultra HD Blu-ray");

            yield return new KeyValuePair<string, string>("web", "Web");
            yield return new KeyValuePair<string, string>("webrip", "Web");
            yield return new KeyValuePair<string, string>("web-dl", "Web");
            yield return new KeyValuePair<string, string>("webdl", "Web");
            yield return new KeyValuePair<string, string>("web-rip", "Web");

            yield return new KeyValuePair<string, string>("hdtv", "HDTV");
            yield return new KeyValuePair<string, string>("pdtv", "HDTV");
            yield return new KeyValuePair<string, string>("dsr", "HDTV");
            yield return new KeyValuePair<string, string>("dsrip", "HDTV");

            yield return new KeyValuePair<string, string>("dvd", "DVD");
            yield return new KeyValuePair<string, string>("dvdrip", "DVD");
            yield return new KeyValuePair<string, string>("dvdscr", "DVD");
            yield return new KeyValuePair<string, string>("dvd-r", "DVD");

            yield return new KeyValuePair<string, string>("hd-dvd", "HD-DVD");
            yield return new KeyValuePair<string, string>("hddvd", "HD-DVD");

            yield return new KeyValuePair<string, string>("sdtv", "TV");
            yield return new KeyValuePair<string, string>("tv", "TV");
            yield return new KeyValuePair<string, string>("tvrip", "TV");
            yield return new KeyValuePair<string, string>("satrip", "Satellite");
            yield return new KeyValuePair<string, string>("satellite", "Satellite");

            yield return new KeyValuePair<string, string>("cam", "Camera");
            yield return new KeyValuePair<string, string>("camrip", "Camera");
            yield return new KeyValuePair<string, string>("hdcam", "HD Camera");

            yield return new KeyValuePair<string, string>("ts", "Telesync");
            yield return new KeyValuePair<string, string>("telesync", "Telesync");
            yield return new KeyValuePair<string, string>("hdts", "HD Telesync");

            yield return new KeyValuePair<string, string>("tc", "Telecine");
            yield return new KeyValuePair<string, string>("telecine", "Telecine");
            yield return new KeyValuePair<string, string>("hdtc", "HD Telecine");

            yield return new KeyValuePair<string, string>("vod", "Video on Demand");
            yield return new KeyValuePair<string, string>("ppv", "Pay-per-view");
            yield return new KeyValuePair<string, string>("workprint", "Workprint");
            yield return new KeyValuePair<string, string>("wp", "Workprint");
            yield return new KeyValuePair<string, string>("vhs", "VHS");
            yield return new KeyValuePair<string, string>("laserdisc", "Laserdisc");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.Source = canonicalValue;
        }
    }
}
