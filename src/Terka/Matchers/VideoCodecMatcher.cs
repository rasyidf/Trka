using System.Collections.Generic;

namespace Terka.Matchers
{
    internal class VideoCodecMatcher : DictionaryMatcher
    {
        protected override IEnumerable<KeyValuePair<string, string>> BuildMap()
        {
            // H.264 / AVC
            yield return new KeyValuePair<string, string>("x264", "H.264");
            yield return new KeyValuePair<string, string>("h264", "H.264");
            yield return new KeyValuePair<string, string>("h.264", "H.264");
            yield return new KeyValuePair<string, string>("avc", "H.264");

            // H.265 / HEVC
            yield return new KeyValuePair<string, string>("x265", "H.265");
            yield return new KeyValuePair<string, string>("h265", "H.265");
            yield return new KeyValuePair<string, string>("h.265", "H.265");
            yield return new KeyValuePair<string, string>("hevc", "H.265");

            // Xvid / DivX
            yield return new KeyValuePair<string, string>("xvid", "Xvid");
            yield return new KeyValuePair<string, string>("divx", "DivX");

            // VP codecs
            yield return new KeyValuePair<string, string>("vp7", "VP7");
            yield return new KeyValuePair<string, string>("vp8", "VP8");
            yield return new KeyValuePair<string, string>("vp9", "VP9");

            // Others
            yield return new KeyValuePair<string, string>("mpeg2", "MPEG-2");
            yield return new KeyValuePair<string, string>("mpeg-2", "MPEG-2");
            yield return new KeyValuePair<string, string>("vc-1", "VC-1");
            yield return new KeyValuePair<string, string>("vc1", "VC-1");
            yield return new KeyValuePair<string, string>("h263", "H.263");
            yield return new KeyValuePair<string, string>("h.263", "H.263");
            yield return new KeyValuePair<string, string>("rv", "RealVideo");
            yield return new KeyValuePair<string, string>("realvideo", "RealVideo");
        }

        protected override void Apply(GuessResult result, string canonicalValue)
        {
            result.VideoCodec = canonicalValue;
        }
    }
}
