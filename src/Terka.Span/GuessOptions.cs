namespace Terka.Span;

/// <summary>
/// Options to influence the guessing process.
/// </summary>
public sealed class GuessOptions
{
    /// <summary>Force the media type instead of auto-detecting.</summary>
    public MediaType? Type { get; set; }
}
