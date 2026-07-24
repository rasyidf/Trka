using System;
using System.Runtime.CompilerServices;

namespace Terka.Span;

/// <summary>
/// Fixed-size inline array of 4 integers. Used for seasons, episodes, and absolute episodes
/// where the vast majority of filenames contain 1–2 values.
/// </summary>
// ponytail: public because exposed via public fields on ZeroAllocGuessResult ref struct.
// Conceptually internal implementation detail; upgrade path: wrap in accessor methods if API surface matters.
[InlineArray(4)]
public struct InlineInt4
{
    private int _element;
}

/// <summary>
/// Fixed-size inline array of 4 strings. Used for editions and other properties
/// where filenames rarely carry more than a few values.
/// </summary>
// ponytail: same as InlineInt4 — public for accessibility, treat as internal.
[InlineArray(4)]
public struct InlineString4
{
    private string _element;
}

/// <summary>
/// Zero-allocation result from <see cref="SpanGuessIt.Guess"/>.
/// All span properties are slices into the original input — no heap strings are created for
/// title or release group. Interned string properties (codecs, sources, etc.) reference
/// pre-existing dictionary values and do not allocate.
/// </summary>
/// <remarks>
/// This is a <see langword="ref struct"/> and cannot be boxed, stored on the heap, or used
/// in async methods. Call <see cref="ToHeapResult"/> to materialize a heap-friendly
/// <see cref="SpanGuessResult"/> when needed.
/// </remarks>
public ref struct ZeroAllocGuessResult
{
    /// <summary>The detected media type (movie, episode, or unknown).</summary>
    public MediaType Type;

    /// <summary>
    /// The title of the media. This is a slice into the original input span — no allocation.
    /// </summary>
    public ReadOnlySpan<char> Title;

    /// <summary>The release year, or <c>null</c> if not detected.</summary>
    public int? Year;

    /// <summary>Inline storage for season numbers.</summary>
    public InlineInt4 Season;

    /// <summary>Number of valid season entries in <see cref="Season"/>.</summary>
    public int SeasonCount;

    /// <summary>Inline storage for episode numbers.</summary>
    public InlineInt4 Episode;

    /// <summary>Number of valid episode entries in <see cref="Episode"/>.</summary>
    public int EpisodeCount;

    /// <summary>Inline storage for absolute (anime-style) episode numbers.</summary>
    public InlineInt4 AbsoluteEpisode;

    /// <summary>Number of valid entries in <see cref="AbsoluteEpisode"/>.</summary>
    public int AbsoluteEpisodeCount;

    /// <summary>Video codec (e.g. "H.264"). Interned from vocabulary — no allocation.</summary>
    public string? VideoCodec;

    /// <summary>Audio codec (e.g. "AAC", "DTS-HD MA"). Interned — no allocation.</summary>
    public string? AudioCodec;

    /// <summary>Audio channel layout (e.g. "5.1", "7.1"). Interned — no allocation.</summary>
    public string? AudioChannels;

    /// <summary>Media source (e.g. "Blu-ray", "HDTV"). Interned — no allocation.</summary>
    public string? Source;

    /// <summary>Screen resolution (e.g. "1080p", "2160p"). Interned — no allocation.</summary>
    public string? ScreenSize;

    /// <summary>Streaming service (e.g. "Netflix", "Disney+"). Interned — no allocation.</summary>
    public string? StreamingService;

    /// <summary>File container extension (e.g. "mkv", "mp4").</summary>
    public string? Container;

    /// <summary>MIME type derived from the container (e.g. "video/x-matroska").</summary>
    public string? Mimetype;

    /// <summary>Color depth (e.g. "10-bit"). Interned — no allocation.</summary>
    public string? ColorDepth;

    /// <summary>Detected language (e.g. "English"). Interned — no allocation.</summary>
    public string? Language;

    /// <summary>
    /// Release/scene group name. This is a slice into the original input span — no allocation.
    /// </summary>
    public ReadOnlySpan<char> ReleaseGroup;

    /// <summary>Inline storage for edition strings (e.g. "Director's Cut").</summary>
    public InlineString4 Edition;

    /// <summary>Number of valid entries in <see cref="Edition"/>.</summary>
    public int EditionCount;

    /// <summary>Inline storage for other properties (e.g. "Remux", "HDR10").</summary>
    public InlineString4 Other;

    /// <summary>Number of valid entries in <see cref="Other"/>.</summary>
    public int OtherCount;

    /// <summary>
    /// Returns <c>true</c> if any season, episode, or absolute episode was detected.
    /// </summary>
    public readonly bool HasEpisodeInfo => SeasonCount > 0 || EpisodeCount > 0 || AbsoluteEpisodeCount > 0;

    /// <summary>
    /// Materializes this stack-only result into a heap-allocated <see cref="SpanGuessResult"/>.
    /// Span fields are converted to <see cref="string"/>; inline arrays are copied to lists.
    /// </summary>
    public readonly SpanGuessResult ToHeapResult()
    {
        var result = new SpanGuessResult
        {
            Type = Type,
            Title = Title.IsEmpty ? null : Title.ToString(),
            Year = Year,
            VideoCodec = VideoCodec,
            AudioCodec = AudioCodec,
            AudioChannels = AudioChannels,
            Source = Source,
            ScreenSize = ScreenSize,
            StreamingService = StreamingService,
            Container = Container,
            Mimetype = Mimetype,
            ColorDepth = ColorDepth,
            Language = Language,
            ReleaseGroup = ReleaseGroup.IsEmpty ? null : ReleaseGroup.ToString(),
        };

        for (var i = 0; i < SeasonCount; i++)
            result.Season.Add(Season[i]);

        for (var i = 0; i < EpisodeCount; i++)
            result.Episode.Add(Episode[i]);

        for (var i = 0; i < AbsoluteEpisodeCount; i++)
            result.AbsoluteEpisode.Add(AbsoluteEpisode[i]);

        for (var i = 0; i < EditionCount; i++)
            result.Edition.Add(Edition[i]);

        for (var i = 0; i < OtherCount; i++)
            result.Other.Add(Other[i]);

        return result;
    }
}
