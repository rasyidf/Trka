using System;
using System.Collections.Generic;

namespace Terka.Shared;

/// <summary>
/// Single source of truth for all media property vocabularies.
/// Both Terka (netstandard2.0) and Terka.Span (net10.0) source from here.
/// 
/// Rules:
/// - Only single-token entries (no separators: no dots, dashes, underscores, spaces)
///   unless the entry is specifically for the base Terka regex matcher.
/// - Multi-word entries go in the base Terka matchers as extras on top of this.
/// </summary>
internal static class Vocabulary
{
    public const int YearMin = 1920;
    public const int YearMax = 2035;

    public static readonly IReadOnlyDictionary<string, string> VideoCodecs =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["x264"] = "H.264",
            ["h264"] = "H.264",
            ["avc"] = "H.264",
            ["x265"] = "H.265",
            ["h265"] = "H.265",
            ["hevc"] = "H.265",
            ["xvid"] = "Xvid",
            ["divx"] = "DivX",
            ["vp7"] = "VP7",
            ["vp8"] = "VP8",
            ["vp9"] = "VP9",
            ["av1"] = "AV1",
            ["mpeg2"] = "MPEG-2",
            ["vc1"] = "VC-1",
            ["h263"] = "H.263",
            ["rv"] = "RealVideo",
            ["realvideo"] = "RealVideo",
        };

    public static readonly IReadOnlyDictionary<string, string> AudioCodecs =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["aac"] = "AAC",
            ["ac3"] = "Dolby Digital",
            ["dd"] = "Dolby Digital",
            ["eac3"] = "Dolby Digital Plus",
            ["ddp"] = "Dolby Digital Plus",
            ["truehd"] = "Dolby TrueHD",
            ["atmos"] = "Dolby Atmos",
            ["dts"] = "DTS",
            ["dtshd"] = "DTS-HD",
            ["dtshdma"] = "DTS-HD",
            ["dtsx"] = "DTS:X",
            ["flac"] = "FLAC",
            ["lpcm"] = "LPCM",
            ["pcm"] = "PCM",
            ["mp2"] = "MP2",
            ["mp3"] = "MP3",
            ["opus"] = "Opus",
            ["vorbis"] = "Vorbis",
        };

    /// <summary>
    /// Two-token audio codec combos (first + second → canonical).
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> AudioCodecsTwoToken =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["dts+hd"] = "DTS-HD",
            ["dts+hdma"] = "DTS-HD",
            ["dts+x"] = "DTS:X",
            ["true+hd"] = "Dolby TrueHD",
            ["dolby+atmos"] = "Dolby Atmos",
        };

    public static readonly IReadOnlyDictionary<string, string> Sources =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["bluray"] = "Blu-ray",
            ["bdrip"] = "Blu-ray",
            ["brrip"] = "Blu-ray",
            ["bdremux"] = "Blu-ray",
            ["web"] = "Web",
            ["webrip"] = "Web",
            ["webdl"] = "Web",
            ["hdtv"] = "HDTV",
            ["pdtv"] = "HDTV",
            ["dsr"] = "HDTV",
            ["dsrip"] = "HDTV",
            ["dvd"] = "DVD",
            ["dvdrip"] = "DVD",
            ["dvdscr"] = "DVD",
            ["hddvd"] = "HD-DVD",
            ["sdtv"] = "TV",
            ["tv"] = "TV",
            ["tvrip"] = "TV",
            ["satrip"] = "Satellite",
            ["satellite"] = "Satellite",
            ["cam"] = "Camera",
            ["camrip"] = "Camera",
            ["hdcam"] = "HD Camera",
            ["ts"] = "Telesync",
            ["telesync"] = "Telesync",
            ["hdts"] = "HD Telesync",
            ["tc"] = "Telecine",
            ["telecine"] = "Telecine",
            ["hdtc"] = "HD Telecine",
            ["vod"] = "Video on Demand",
            ["ppv"] = "Pay-per-view",
            ["workprint"] = "Workprint",
            ["wp"] = "Workprint",
            ["vhs"] = "VHS",
            ["laserdisc"] = "Laserdisc",
            ["uhdbluray"] = "Ultra HD Blu-ray",
        };

    public static readonly IReadOnlyDictionary<string, string> Editions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["dc"] = "Director's Cut",
            ["extended"] = "Extended",
            ["unrated"] = "Unrated",
            ["uncut"] = "Uncut",
            ["remastered"] = "Remastered",
            ["restored"] = "Restored",
            ["theatrical"] = "Theatrical",
            ["imax"] = "IMAX",
            ["special"] = "Special",
            ["limited"] = "Limited",
            ["collector"] = "Collector",
            ["criterion"] = "Criterion",
            ["ultimate"] = "Ultimate",
            ["deluxe"] = "Deluxe",
            ["uncensored"] = "Uncensored",
            ["fan"] = "Fan",
            ["festival"] = "Festival",
        };

    public static readonly IReadOnlyDictionary<string, string> Others =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["proper"] = "Proper",
            ["repack"] = "Proper",
            ["rerip"] = "Proper",
            ["remux"] = "Remux",
            ["3d"] = "3D",
            ["hdr"] = "HDR10",
            ["hdr10"] = "HDR10",
            ["dv"] = "Dolby Vision",
            ["dolbyvision"] = "Dolby Vision",
            ["dual"] = "Dual Audio",
            ["complete"] = "Complete",
            ["internal"] = "Internal",
            ["sample"] = "Sample",
            ["fix"] = "Fix",
            ["dubbed"] = "Line Dubbed",
            ["dub"] = "Line Dubbed",
            ["screener"] = "Screener",
            ["scr"] = "Screener",
            ["trailer"] = "Trailer",
            ["hybrid"] = "Hybrid",
            ["uhd"] = "Ultra HD",
            ["hd"] = "HD",
            ["fullhd"] = "Full HD",
            ["hardcoded"] = "Hardcoded Subtitles",
            ["hc"] = "Hardcoded Subtitles",
            ["subbed"] = "Hardcoded Subtitles",
            ["documentary"] = "Documentary",
            ["doc"] = "Documentary",
            ["xxx"] = "XXX",
        };

    public static readonly IReadOnlyDictionary<string, string> StreamingServices =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["amzn"] = "Amazon Prime",
            ["amazon"] = "Amazon Prime",
            ["nf"] = "Netflix",
            ["netflix"] = "Netflix",
            ["dsnp"] = "Disney+",
            ["disneyplus"] = "Disney+",
            ["hulu"] = "Hulu",
            ["hmax"] = "HBO Max",
            ["hbo"] = "HBO Max",
            ["atvp"] = "AppleTV",
            ["aptv"] = "AppleTV",
            ["pcok"] = "Peacock",
            ["peacock"] = "Peacock",
            ["pmtp"] = "Paramount+",
            ["cr"] = "Crunchy Roll",
            ["crunchyroll"] = "Crunchy Roll",
            ["stan"] = "Stan",
            ["crav"] = "Crave",
            ["crave"] = "Crave",
            ["sho"] = "Showtime",
            ["showtime"] = "Showtime",
            ["starz"] = "Starz",
            ["max"] = "Max",
            ["binge"] = "Binge",
            ["it"] = "iTunes",
            ["itunes"] = "iTunes",
            ["red"] = "YouTube Red",
            ["yt"] = "YouTube Red",
            ["iqiyi"] = "iQIYI",
        };

    public static readonly IReadOnlyDictionary<string, string> ColorDepths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["8bit"] = "8-bit",
            ["10bit"] = "10-bit",
            ["12bit"] = "12-bit",
        };

    public static readonly IReadOnlyDictionary<string, string> Languages =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["german"] = "German",
            ["french"] = "French",
            ["english"] = "English",
            ["spanish"] = "Spanish",
            ["italian"] = "Italian",
            ["japanese"] = "Japanese",
            ["chinese"] = "Chinese",
            ["korean"] = "Korean",
            ["russian"] = "Russian",
            ["portuguese"] = "Portuguese",
            ["dutch"] = "Dutch",
            ["hindi"] = "Hindi",
            ["arabic"] = "Arabic",
            ["thai"] = "Thai",
            ["swedish"] = "Swedish",
            ["norwegian"] = "Norwegian",
            ["danish"] = "Danish",
            ["finnish"] = "Finnish",
            ["polish"] = "Polish",
            ["turkish"] = "Turkish",
            ["czech"] = "Czech",
            ["multi"] = "Multi",
            ["dl"] = "Dual Language",
            ["ger"] = "German",
            ["fre"] = "French",
            ["eng"] = "English",
            ["spa"] = "Spanish",
            ["ita"] = "Italian",
            ["jpn"] = "Japanese",
            ["chi"] = "Chinese",
            ["kor"] = "Korean",
            ["rus"] = "Russian",
            ["por"] = "Portuguese",
            ["dut"] = "Dutch",
            ["hin"] = "Hindi",
        };

    public static readonly IReadOnlyDictionary<string, string> Countries =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["us"] = "US",
            ["uk"] = "UK",
            ["au"] = "AU",
            ["nz"] = "NZ",
            ["ca"] = "CA",
            ["de"] = "DE",
            ["fr"] = "FR",
            ["jp"] = "JP",
            ["kr"] = "KR",
            ["cn"] = "CN",
            ["br"] = "BR",
            ["in"] = "IN",
            ["es"] = "ES",
            ["it"] = "IT",
            ["ru"] = "RU",
            ["nl"] = "NL",
            ["se"] = "SE",
        };

    /// <summary>
    /// Maps container extension (without dot) to MIME type.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> Containers =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["mkv"] = "video/x-matroska",
            ["avi"] = "video/x-msvideo",
            ["mp4"] = "video/mp4",
            ["m4v"] = "video/mp4",
            ["mov"] = "video/quicktime",
            ["wmv"] = "video/x-ms-wmv",
            ["flv"] = "video/x-flv",
            ["webm"] = "video/webm",
            ["ogv"] = "video/ogg",
            ["mpg"] = "video/mpeg",
            ["mpeg"] = "video/mpeg",
            ["vob"] = "video/dvd",
            ["m2ts"] = "video/mp2t",
            ["ts"] = "video/mp2t",
            ["3gp"] = "video/3gpp",
        };

    /// <summary>
    /// All known media file extensions (without dot).
    /// </summary>
    public static readonly IReadOnlyCollection<string> KnownExtensions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "3g2", "3gp", "avi", "asf", "divx", "flv", "m2ts", "m4v",
            "mk3d", "mka", "mkv", "mov", "mp4", "mpeg", "mpg", "ogg",
            "ogm", "ogv", "rm", "srt", "ssa", "ts", "vob", "wav",
            "webm", "wmv",
        };

    /// <summary>
    /// Canonical screen size strings.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> ScreenSizes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["240p"] = "240p",
            ["360p"] = "360p",
            ["480p"] = "480p",
            ["480i"] = "480i",
            ["540p"] = "540p",
            ["540i"] = "540i",
            ["576p"] = "576p",
            ["576i"] = "576i",
            ["720p"] = "720p",
            ["720i"] = "720i",
            ["900p"] = "900p",
            ["900i"] = "900i",
            ["1080p"] = "1080p",
            ["1080i"] = "1080i",
            ["1440p"] = "1440p",
            ["2160p"] = "2160p",
            ["4320p"] = "4320p",
        };

    /// <summary>
    /// Audio channel configurations (single-token matches).
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> AudioChannels =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["7.1"] = "7.1",
            ["5.1"] = "5.1",
            ["2.0"] = "2.0",
            ["1.0"] = "1.0",
            ["stereo"] = "2.0",
            ["mono"] = "1.0",
        };

    /// <summary>
    /// Two-token audio channel combos (first digit + second digit → canonical).
    /// </summary>
    public static readonly (string First, string Second, string Channels)[] AudioChannelsTwoToken =
    [
        ("7", "1", "7.1"),
        ("5", "1", "5.1"),
        ("2", "0", "2.0"),
        ("1", "0", "1.0")
    ];
}
