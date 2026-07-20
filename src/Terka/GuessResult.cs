using System.Collections.Generic;

namespace Terka
{
    /// <summary>
    /// Result of guessing properties from a media filename.
    /// Mirrors the Python guessit output schema.
    /// </summary>
    public class GuessResult
    {
        public MediaType Type { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }

        // Episode info
        public List<int> Season { get; set; }
        public List<int> Episode { get; set; }
        public List<int> AbsoluteEpisode { get; set; }
        public string EpisodeTitle { get; set; }

        // Quality / source
        public string Source { get; set; }
        public string ScreenSize { get; set; }
        public string VideoCodec { get; set; }
        public string VideoProfile { get; set; }
        public string AudioCodec { get; set; }
        public string AudioChannels { get; set; }
        public string AudioProfile { get; set; }

        // Release info
        public string ReleaseGroup { get; set; }
        public string Container { get; set; }
        public string Mimetype { get; set; }
        public List<string> Edition { get; set; }
        public List<string> Other { get; set; }

        // Misc
        public string Country { get; set; }
        public string Language { get; set; }
        public string SubtitleLanguage { get; set; }
        public string Date { get; set; }
        public string Website { get; set; }
        public string StreamingService { get; set; }
        public string Crc32 { get; set; }
        public string ColorDepth { get; set; }
        public string ScanType { get; set; }

        public GuessResult()
        {
            Season = new List<int>();
            Episode = new List<int>();
            AbsoluteEpisode = new List<int>();
            Edition = new List<string>();
            Other = new List<string>();
        }

        /// <summary>
        /// Returns a dictionary representation, omitting null/empty values.
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();

            dict["type"] = Type == MediaType.Episode ? "episode" : "movie";

            if (!string.IsNullOrEmpty(Title)) dict["title"] = Title;
            if (Year.HasValue) dict["year"] = Year.Value;

            if (Season.Count == 1) dict["season"] = Season[0];
            else if (Season.Count > 1) dict["season"] = Season;

            if (Episode.Count == 1) dict["episode"] = Episode[0];
            else if (Episode.Count > 1) dict["episode"] = Episode;

            if (AbsoluteEpisode.Count == 1) dict["absolute_episode"] = AbsoluteEpisode[0];
            else if (AbsoluteEpisode.Count > 1) dict["absolute_episode"] = AbsoluteEpisode;

            if (!string.IsNullOrEmpty(EpisodeTitle)) dict["episode_title"] = EpisodeTitle;
            if (!string.IsNullOrEmpty(Source)) dict["source"] = Source;
            if (!string.IsNullOrEmpty(ScreenSize)) dict["screen_size"] = ScreenSize;
            if (!string.IsNullOrEmpty(VideoCodec)) dict["video_codec"] = VideoCodec;
            if (!string.IsNullOrEmpty(VideoProfile)) dict["video_profile"] = VideoProfile;
            if (!string.IsNullOrEmpty(AudioCodec)) dict["audio_codec"] = AudioCodec;
            if (!string.IsNullOrEmpty(AudioChannels)) dict["audio_channels"] = AudioChannels;
            if (!string.IsNullOrEmpty(AudioProfile)) dict["audio_profile"] = AudioProfile;
            if (!string.IsNullOrEmpty(ReleaseGroup)) dict["release_group"] = ReleaseGroup;
            if (!string.IsNullOrEmpty(Container)) dict["container"] = Container;
            if (!string.IsNullOrEmpty(Mimetype)) dict["mimetype"] = Mimetype;

            if (Edition.Count == 1) dict["edition"] = Edition[0];
            else if (Edition.Count > 1) dict["edition"] = Edition;

            if (Other.Count == 1) dict["other"] = Other[0];
            else if (Other.Count > 1) dict["other"] = Other;

            if (!string.IsNullOrEmpty(Country)) dict["country"] = Country;
            if (!string.IsNullOrEmpty(Language)) dict["language"] = Language;
            if (!string.IsNullOrEmpty(SubtitleLanguage)) dict["subtitle_language"] = SubtitleLanguage;
            if (!string.IsNullOrEmpty(Date)) dict["date"] = Date;
            if (!string.IsNullOrEmpty(Website)) dict["website"] = Website;
            if (!string.IsNullOrEmpty(StreamingService)) dict["streaming_service"] = StreamingService;
            if (!string.IsNullOrEmpty(Crc32)) dict["crc32"] = Crc32;
            if (!string.IsNullOrEmpty(ColorDepth)) dict["color_depth"] = ColorDepth;
            if (!string.IsNullOrEmpty(ScanType)) dict["scan_type"] = ScanType;

            return dict;
        }
    }
}
