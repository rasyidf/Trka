using System.Collections.Generic;

namespace Terka.Span;

/// <summary>
/// Result of guessing properties from a media filename.
/// Lists are lazy-initialized to avoid allocations when unused.
/// </summary>
public sealed class SpanGuessResult
{
    public MediaType Type { get; set; }
    public string? Title { get; set; }
    public int? Year { get; set; }

    // ponytail: Lazy-init lists — most filenames use 0-1 seasons, 0-2 episodes.
    // Saves 5 List<T> backing-array allocations per Guess() call for the common case.
    private List<int>? _season;
    private List<int>? _episode;
    private List<int>? _absoluteEpisode;
    private List<string>? _edition;
    private List<string>? _other;

    public List<int> Season => _season ??= new();
    public List<int> Episode => _episode ??= new();
    public List<int> AbsoluteEpisode => _absoluteEpisode ??= new();
    public List<string> Edition => _edition ??= new();
    public List<string> Other => _other ??= new();

    // Read-only access that doesn't trigger allocation
    internal bool HasSeason => _season is { Count: > 0 };
    internal bool HasEpisode => _episode is { Count: > 0 };
    internal bool HasAbsoluteEpisode => _absoluteEpisode is { Count: > 0 };

    public string? EpisodeTitle { get; set; }

    public string? Source { get; set; }
    public string? ScreenSize { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public string? AudioChannels { get; set; }

    public string? ReleaseGroup { get; set; }
    public string? Container { get; set; }
    public string? Mimetype { get; set; }

    public string? StreamingService { get; set; }
    public string? ColorDepth { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public string? Crc32 { get; set; }
    public float Confidence { get; set; }

    /// <summary>
    /// Returns a dictionary representation, omitting null/empty values.
    /// Matches the output format of <see cref="Terka.GuessResult.ToDictionary"/>.
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();

        dict["type"] = Type == MediaType.Episode ? "episode" : "movie";

        if (!string.IsNullOrEmpty(Title)) dict["title"] = Title;
        if (Year.HasValue) dict["year"] = Year.Value;

        if (HasSeason)
        {
            if (_season!.Count == 1) dict["season"] = _season[0];
            else dict["season"] = _season;
        }
        if (HasEpisode)
        {
            if (_episode!.Count == 1) dict["episode"] = _episode[0];
            else dict["episode"] = _episode;
        }
        if (HasAbsoluteEpisode)
        {
            if (_absoluteEpisode!.Count == 1) dict["absolute_episode"] = _absoluteEpisode[0];
            else dict["absolute_episode"] = _absoluteEpisode;
        }

        if (!string.IsNullOrEmpty(EpisodeTitle)) dict["episode_title"] = EpisodeTitle;
        if (!string.IsNullOrEmpty(Source)) dict["source"] = Source;
        if (!string.IsNullOrEmpty(ScreenSize)) dict["screen_size"] = ScreenSize;
        if (!string.IsNullOrEmpty(VideoCodec)) dict["video_codec"] = VideoCodec;
        if (!string.IsNullOrEmpty(AudioCodec)) dict["audio_codec"] = AudioCodec;
        if (!string.IsNullOrEmpty(AudioChannels)) dict["audio_channels"] = AudioChannels;
        if (!string.IsNullOrEmpty(ReleaseGroup)) dict["release_group"] = ReleaseGroup;
        if (!string.IsNullOrEmpty(Container)) dict["container"] = Container;
        if (!string.IsNullOrEmpty(Mimetype)) dict["mimetype"] = Mimetype;
        if (!string.IsNullOrEmpty(StreamingService)) dict["streaming_service"] = StreamingService;
        if (!string.IsNullOrEmpty(ColorDepth)) dict["color_depth"] = ColorDepth;
        if (!string.IsNullOrEmpty(Country)) dict["country"] = Country;
        if (!string.IsNullOrEmpty(Language)) dict["language"] = Language;
        if (Confidence > 0f) dict["confidence"] = Confidence;

        if (_edition is { Count: > 0 })
        {
            if (_edition.Count == 1) dict["edition"] = _edition[0];
            else dict["edition"] = _edition;
        }
        if (_other is { Count: > 0 })
        {
            if (_other.Count == 1) dict["other"] = _other[0];
            else dict["other"] = _other;
        }

        return dict;
    }
}

public enum MediaType { Unknown, Movie, Episode }
