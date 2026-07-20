using System.Collections.Generic;

namespace Terka.Span;

/// <summary>
/// Result of guessing properties from a media filename.
/// Same shape as Terka.GuessResult but designed for use with the Span-based parser.
/// </summary>
public sealed class SpanGuessResult
{
    public MediaType Type { get; set; }
    public string? Title { get; set; }
    public int? Year { get; set; }

    public List<int> Season { get; } = new();
    public List<int> Episode { get; } = new();
    public List<int> AbsoluteEpisode { get; } = new();
    public string? EpisodeTitle { get; set; }

    public string? Source { get; set; }
    public string? ScreenSize { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public string? AudioChannels { get; set; }

    public string? ReleaseGroup { get; set; }
    public string? Container { get; set; }
    public string? Mimetype { get; set; }
    public List<string> Edition { get; } = new();
    public List<string> Other { get; } = new();

    public string? StreamingService { get; set; }
    public string? ColorDepth { get; set; }
}

public enum MediaType { Unknown, Movie, Episode }
