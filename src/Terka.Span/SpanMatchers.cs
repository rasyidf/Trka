using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terka.Shared;

namespace Terka.Span;

/// <summary>
/// High-performance dictionary matcher using FrozenDictionary with AlternateLookup.
/// Zero-allocation span-based lookups — no string created per match attempt.
/// All dictionaries sourced from <see cref="Vocabulary"/> (single source of truth).
/// </summary>
internal static class SpanDictLookup
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Lookup(ReadOnlySpan<char> token, FrozenDictionary<string, string> map)
    {
        var alt = map.GetAlternateLookup<ReadOnlySpan<char>>();
        return alt.TryGetValue(token, out var value) ? value : null;
    }

    /// <summary>
    /// Convert an IReadOnlyDictionary to a FrozenDictionary (case-insensitive).
    /// </summary>
    internal static FrozenDictionary<string, string> Freeze(IReadOnlyDictionary<string, string> source) =>
        source.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
}

internal static class SpanVideoCodec
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.VideoCodecs);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanAudioCodec
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.AudioCodecs);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);

    public static string? MatchTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        // ponytail: Inline two-token matching instead of dictionary concat to avoid allocation.
        if (first.Equals("dts", StringComparison.OrdinalIgnoreCase))
        {
            if (second.Equals("hd", StringComparison.OrdinalIgnoreCase) ||
                second.Equals("hdma", StringComparison.OrdinalIgnoreCase)) return "DTS-HD";
            if (second.Equals("x", StringComparison.OrdinalIgnoreCase)) return "DTS:X";
        }
        if (first.Equals("true", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("hd", StringComparison.OrdinalIgnoreCase)) return "Dolby TrueHD";
        if (first.Equals("dolby", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("atmos", StringComparison.OrdinalIgnoreCase)) return "Dolby Atmos";
        return null;
    }
}

internal static class SpanSource
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.Sources);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);

    public static string? MatchTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        // Two-token source patterns that the tokenizer splits on separators
        if (first.Equals("blu", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("ray", StringComparison.OrdinalIgnoreCase)) return "Blu-ray";
        if (first.Equals("web", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("dl", StringComparison.OrdinalIgnoreCase)) return "Web";
        if (first.Equals("web", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("rip", StringComparison.OrdinalIgnoreCase)) return "Web";
        if (first.Equals("hd", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("dvd", StringComparison.OrdinalIgnoreCase)) return "HD-DVD";
        if (first.Equals("uhd", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("bluray", StringComparison.OrdinalIgnoreCase)) return "Ultra HD Blu-ray";
        return null;
    }
}

internal static class SpanEdition
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.Editions);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);

    public static string? MatchTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        if (second.Equals("cut", StringComparison.OrdinalIgnoreCase))
        {
            if (first.Equals("directors", StringComparison.OrdinalIgnoreCase) ||
                first.Equals("director's", StringComparison.OrdinalIgnoreCase)) return "Director's Cut";
            if (first.Equals("extended", StringComparison.OrdinalIgnoreCase)) return "Extended";
            if (first.Equals("theatrical", StringComparison.OrdinalIgnoreCase)) return "Theatrical";
            if (first.Equals("alternative", StringComparison.OrdinalIgnoreCase)) return "Alternative Cut";
            if (first.Equals("final", StringComparison.OrdinalIgnoreCase)) return "Final Cut";
        }
        if (first.Equals("special", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("edition", StringComparison.OrdinalIgnoreCase)) return "Special";
        if (first.Equals("fan", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("edit", StringComparison.OrdinalIgnoreCase)) return "Fan";
        return null;
    }
}

internal static class SpanOther
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.Others);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);

    public static string? MatchTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        if (first.Equals("dolby", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("vision", StringComparison.OrdinalIgnoreCase)) return "Dolby Vision";
        if (first.Equals("dual", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("audio", StringComparison.OrdinalIgnoreCase)) return "Dual Audio";
        if (first.Equals("open", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("matte", StringComparison.OrdinalIgnoreCase)) return "Open Matte";
        if (first.Equals("hdr10", StringComparison.OrdinalIgnoreCase) &&
            second.Equals("+", StringComparison.Ordinal)) return "HDR10";
        return null;
    }
}

internal static class SpanStreamingService
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.StreamingServices);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanColorDepth
{
    private static readonly FrozenDictionary<string, string> Map = SpanDictLookup.Freeze(Vocabulary.ColorDepths);
    public static string? Match(ReadOnlySpan<char> token) => SpanDictLookup.Lookup(token, Map);
}

internal static class SpanContainer
{
    private static readonly FrozenDictionary<string, string> MimeTypes = SpanDictLookup.Freeze(Vocabulary.Containers);

    // ponytail: Maps ext → canonical lowercase ext for zero-alloc container name return.
    // Built from KnownExtensions collection.
    private static readonly FrozenDictionary<string, string> Known =
        ((HashSet<string>)Vocabulary.KnownExtensions)
            .ToFrozenDictionary(e => e, e => e.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase);

    public static string? TryGetCanonical(ReadOnlySpan<char> ext)
    {
        var alt = Known.GetAlternateLookup<ReadOnlySpan<char>>();
        return alt.TryGetValue(ext, out var canonical) ? canonical : null;
    }

    public static bool IsKnown(ReadOnlySpan<char> ext)
    {
        var alt = Known.GetAlternateLookup<ReadOnlySpan<char>>();
        return alt.ContainsKey(ext);
    }

    public static string? GetMime(ReadOnlySpan<char> ext)
    {
        var alt = MimeTypes.GetAlternateLookup<ReadOnlySpan<char>>();
        return alt.TryGetValue(ext, out var m) ? m : null;
    }
}

internal static class SpanScreenSize
{
    private static readonly FrozenDictionary<string, string> Canonical = SpanDictLookup.Freeze(Vocabulary.ScreenSizes);

    /// <summary>
    /// Returns the canonical lowercase screen-size string, or null if not a screen size.
    /// </summary>
    public static string? Match(ReadOnlySpan<char> slice)
    {
        if (slice.Length is < 4 or > 9) return null;

        var last = slice[^1];
        if (last is 'p' or 'i' or 'P' or 'I')
        {
            for (var i = 0; i < slice.Length - 1; i++)
                if (!char.IsDigit(slice[i])) return null;

            // Try canonical lookup first (zero alloc for common sizes)
            var alt = Canonical.GetAlternateLookup<ReadOnlySpan<char>>();
            if (alt.TryGetValue(slice, out var canonical)) return canonical;

            // ponytail: Fallback for exotic resolutions like "1920p". One alloc, rare path.
            Span<char> buf = stackalloc char[slice.Length];
            for (var i = 0; i < slice.Length; i++)
                buf[i] = char.ToLowerInvariant(slice[i]);
            return new string(buf);
        }

        // NxN format (e.g. 1920x1080)
        var x = slice.IndexOf('x');
        if (x < 0) x = slice.IndexOf('X');
        if (x <= 0 || x >= slice.Length - 1) return null;
        
        {
            for (var i = 0; i < x; i++)
                if (!char.IsDigit(slice[i])) return null;
            for (var i = x + 1; i < slice.Length; i++)
                if (!char.IsDigit(slice[i])) return null;

            Span<char> buf = stackalloc char[slice.Length];
            for (var i = 0; i < slice.Length; i++)
                buf[i] = (slice[i] == 'X') ? 'x' : slice[i];
            return new string(buf);
        }

    }
}
