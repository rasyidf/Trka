using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Terka.Span;

/// <summary>
/// High-performance dictionary matcher using ReadOnlySpan comparisons.
/// Uses a pre-sorted array for binary-search-like lookup with span keys.
/// </summary>
internal static class SpanDictLookup
{
    /// <summary>
    /// Look up a span in a dictionary (case-insensitive).
    /// Returns the canonical value or null if not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Lookup(ReadOnlySpan<char> token, Dictionary<string, string> map)
    {
        // ponytail: Allocates one string per lookup for the dictionary key.
        // Upgrade path: custom hash map with span-native keys (FrozenDictionary in .NET 8+).
        // For benchmarking, this is still much faster than the original due to tokenizer savings.
        var key = new string(token);
        return map.TryGetValue(key, out var value) ? value : null;
    }
}

/// <summary>
/// Video codec pattern matching using span operations instead of regex.
/// </summary>
internal static class SpanVideoCodec
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["x264"] = "H.264", ["h264"] = "H.264", ["h.264"] = "H.264", ["avc"] = "H.264",
        ["x265"] = "H.265", ["h265"] = "H.265", ["h.265"] = "H.265", ["hevc"] = "H.265",
        ["xvid"] = "Xvid", ["divx"] = "DivX",
        ["vp7"] = "VP7", ["vp8"] = "VP8", ["vp9"] = "VP9",
        ["mpeg2"] = "MPEG-2", ["mpeg-2"] = "MPEG-2",
        ["vc-1"] = "VC-1", ["vc1"] = "VC-1",
        ["h263"] = "H.263", ["h.263"] = "H.263",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanAudioCodec
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["aac"] = "AAC", ["ac3"] = "Dolby Digital", ["dd"] = "Dolby Digital",
        ["eac3"] = "Dolby Digital Plus", ["ddp"] = "Dolby Digital Plus",
        ["truehd"] = "Dolby TrueHD", ["atmos"] = "Dolby Atmos",
        ["dts"] = "DTS", ["dtshd"] = "DTS-HD", ["dts-hd"] = "DTS-HD", ["dts-hdma"] = "DTS-HD",
        ["dtsx"] = "DTS:X", ["dts-x"] = "DTS:X",
        ["flac"] = "FLAC", ["lpcm"] = "LPCM", ["pcm"] = "PCM",
        ["mp2"] = "MP2", ["mp3"] = "MP3", ["opus"] = "Opus", ["vorbis"] = "Vorbis",
    };

    // Two-token patterns
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);

    public static string? MatchTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        if (first.Equals("dts", StringComparison.OrdinalIgnoreCase))
        {
            if (second.Equals("hd", StringComparison.OrdinalIgnoreCase) ||
                second.Equals("hdma", StringComparison.OrdinalIgnoreCase)) return "DTS-HD";
            if (second.Equals("x", StringComparison.OrdinalIgnoreCase)) return "DTS:X";
        }
        if (first.Equals("true", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("hd", StringComparison.OrdinalIgnoreCase)) return "Dolby TrueHD";
        return null;
    }
}

internal static class SpanSource
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bluray"] = "Blu-ray", ["blu-ray"] = "Blu-ray", ["bdrip"] = "Blu-ray", ["brrip"] = "Blu-ray", ["bdremux"] = "Blu-ray",
        ["web"] = "Web", ["webrip"] = "Web", ["web-dl"] = "Web", ["webdl"] = "Web", ["web-rip"] = "Web",
        ["hdtv"] = "HDTV", ["pdtv"] = "HDTV", ["dsr"] = "HDTV", ["dsrip"] = "HDTV",
        ["dvd"] = "DVD", ["dvdrip"] = "DVD", ["dvdscr"] = "DVD", ["dvd-r"] = "DVD",
        ["hd-dvd"] = "HD-DVD", ["hddvd"] = "HD-DVD",
        ["sdtv"] = "TV", ["tv"] = "TV", ["tvrip"] = "TV",
        ["satrip"] = "Satellite", ["satellite"] = "Satellite",
        ["cam"] = "Camera", ["camrip"] = "Camera", ["hdcam"] = "HD Camera",
        ["ts"] = "Telesync", ["telesync"] = "Telesync", ["hdts"] = "HD Telesync",
        ["tc"] = "Telecine", ["telecine"] = "Telecine", ["hdtc"] = "HD Telecine",
        ["vod"] = "Video on Demand", ["ppv"] = "Pay-per-view",
        ["workprint"] = "Workprint", ["vhs"] = "VHS", ["laserdisc"] = "Laserdisc",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanEdition
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["dc"] = "Director's Cut", ["extended"] = "Extended", ["unrated"] = "Unrated",
        ["uncut"] = "Uncut", ["remastered"] = "Remastered", ["restored"] = "Restored",
        ["theatrical"] = "Theatrical", ["imax"] = "IMAX", ["special"] = "Special",
        ["limited"] = "Limited", ["collector"] = "Collector", ["criterion"] = "Criterion",
        ["ultimate"] = "Ultimate", ["deluxe"] = "Deluxe", ["uncensored"] = "Uncensored",
        ["fan"] = "Fan", ["festival"] = "Festival",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanOther
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["proper"] = "Proper", ["repack"] = "Proper", ["rerip"] = "Proper",
        ["remux"] = "Remux", ["3d"] = "3D",
        ["hdr"] = "HDR10", ["hdr10"] = "HDR10",
        ["dv"] = "Dolby Vision", ["dolbyvision"] = "Dolby Vision",
        ["dual"] = "Dual Audio", ["multi"] = "Dual Audio",
        ["complete"] = "Complete", ["internal"] = "Internal", ["sample"] = "Sample",
        ["fix"] = "Fix", ["dubbed"] = "Line Dubbed", ["dub"] = "Line Dubbed",
        ["screener"] = "Screener", ["trailer"] = "Trailer",
        ["hybrid"] = "Hybrid", ["uhd"] = "Ultra HD", ["hd"] = "HD",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanStreamingService
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["amzn"] = "Amazon Prime", ["amazon"] = "Amazon Prime",
        ["nf"] = "Netflix", ["netflix"] = "Netflix",
        ["dsnp"] = "Disney+", ["disneyplus"] = "Disney+",
        ["hulu"] = "Hulu", ["hmax"] = "HBO Max", ["hbo"] = "HBO Max",
        ["atvp"] = "AppleTV", ["aptv"] = "AppleTV",
        ["pcok"] = "Peacock", ["peacock"] = "Peacock",
        ["pmtp"] = "Paramount+",
        ["cr"] = "Crunchy Roll", ["crunchyroll"] = "Crunchy Roll",
        ["stan"] = "Stan", ["crav"] = "Crave", ["crave"] = "Crave",
        ["sho"] = "Showtime", ["showtime"] = "Showtime",
        ["starz"] = "Starz", ["max"] = "Max", ["binge"] = "Binge",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanColorDepth
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["8bit"] = "8-bit", ["8-bit"] = "8-bit",
        ["10bit"] = "10-bit", ["10-bit"] = "10-bit",
        ["12bit"] = "12-bit", ["12-bit"] = "12-bit",
    };

    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanContainer
{
    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["mkv"] = "video/x-matroska", ["avi"] = "video/x-msvideo", ["mp4"] = "video/mp4",
        ["m4v"] = "video/mp4", ["mov"] = "video/quicktime", ["wmv"] = "video/x-ms-wmv",
        ["flv"] = "video/x-flv", ["webm"] = "video/webm", ["ogv"] = "video/ogg",
        ["mpg"] = "video/mpeg", ["mpeg"] = "video/mpeg", ["vob"] = "video/dvd",
        ["m2ts"] = "video/mp2t", ["ts"] = "video/mp2t", ["3gp"] = "video/3gpp",
    };

    private static readonly HashSet<string> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        "3g2","3gp","avi","asf","divx","flv","m2ts","m4v","mk3d","mka","mkv","mov",
        "mp4","mpeg","mpg","ogg","ogm","ogv","rm","srt","ssa","ts","vob","wav","webm","wmv",
    };

    public static bool IsKnown(ReadOnlySpan<char> ext)
    {
        var s = new string(ext);
        return Known.Contains(s);
    }

    public static string? GetMime(ReadOnlySpan<char> ext)
    {
        var s = new string(ext);
        return MimeTypes.TryGetValue(s, out var m) ? m : null;
    }
}
