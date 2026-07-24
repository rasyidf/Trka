using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terka.Shared;

namespace Terka.Span;

/// <summary>
/// High-performance span-based media filename parser.
/// Minimizes allocations by operating on ReadOnlySpan&lt;char&gt; slices.
/// All vocabularies sourced from <see cref="Vocabulary"/>.
/// </summary>
public static class SpanGuessIt
{
    private static readonly FrozenDictionary<string, string> LanguageMap =
        Vocabulary.Languages.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Guess media properties from a filename using span-based parsing.
    /// </summary>
    public static SpanGuessResult Guess(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));

        return Guess(filename.AsSpan());
    }

    /// <summary>
    /// Guess media properties from a filename with options.
    /// </summary>
    public static SpanGuessResult Guess(ReadOnlySpan<char> input, GuessOptions? options)
    {
        var result = Guess(input);
        if (options?.Type != null)
            result.Type = options.Type.Value;
        return result;
    }

    /// <summary>
    /// Guess media properties from a filename string with options.
    /// </summary>
    public static SpanGuessResult Guess(string filename, GuessOptions? options)
    {
        return string.IsNullOrWhiteSpace(filename) ? throw new ArgumentException("Filename cannot be null or empty.", nameof(filename)) : Guess(filename.AsSpan(), options);
    }

    /// <summary>
    /// Attempts to guess media properties. Returns false if the input doesn't appear to be a media filename.
    /// </summary>
    public static bool TryGuess(ReadOnlySpan<char> input, out SpanGuessResult? result)
    {
        result = null;
        if (input.IsEmpty || input.Length < 3) return false;

        // Must have at least one separator or bracket to be a structured media filename
        var hasStructure = false;
        foreach (var c in input)
        {
            if (c is not ('.' or ' ' or '_' or '-' or '[' or '(')) continue;
            hasStructure = true; break;
        }
        if (!hasStructure) return false;

        var r = Guess(input);
        // A valid media filename should have at least a title and one other property
        if (r.Title == null || (r.VideoCodec == null && r.Source == null && r.ScreenSize == null && !r.HasSeason && !r.HasEpisode && r.Year == null))
            return false;

        result = r;
        return true;
    }

    /// <summary>
    /// Attempts to guess media properties from a string. Returns false if the input doesn't appear to be a media filename.
    /// </summary>
    public static bool TryGuess(string filename, out SpanGuessResult? result)
    {
        result = null;
        return !string.IsNullOrWhiteSpace(filename) && TryGuess(filename.AsSpan(), out result);
    }

    /// <summary>
    /// Guess media properties from a filename span.
    /// </summary>
    public static SpanGuessResult Guess(ReadOnlySpan<char> input)
    {
        var result = new SpanGuessResult();

        // Extract extension — zero-alloc canonical container lookup
        var ext = SpanTokenizer.GetExtension(input);
        if (!ext.IsEmpty)
        {
            var container = SpanContainer.TryGetCanonical(ext);
            if (container != null)
            {
                result.Container = container;
                result.Mimetype = SpanContainer.GetMime(ext);
            }
        }

        // Get the name without path/extension
        var name = SpanTokenizer.GetFileNameWithoutExtension(input);

        // Tokenize onto the stack
        Span<SpanToken> tokens = stackalloc SpanToken[SpanTokenizer.MaxTokens];
        var tokenCount = SpanTokenizer.TokenizeName(name, tokens);

        // Episode matching (S##E## across joined tokens)
        MatchEpisodes(name, tokens, tokenCount, result);

        // Single-token property matching
        for (var i = 0; i < tokenCount; i++)
        {
            ref var t = ref tokens[i];
            if (t.Matched) continue;

            var slice = t.Slice(name);

            // Video codec
            if (result.VideoCodec == null)
            {
                var v = SpanVideoCodec.Match(slice);
                if (v != null) { result.VideoCodec = v; t.Matched = true; continue; }
            }

            // Audio codec (two-token lookahead)
            if (result.AudioCodec == null)
            {
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var next = tokens[i + 1].Slice(name);
                    var twoMatch = SpanAudioCodec.MatchTwo(slice, next);
                    if (twoMatch != null)
                    {
                        result.AudioCodec = twoMatch;
                        t.Matched = true;
                        tokens[i + 1].Matched = true;
                        continue;
                    }
                }
                var a = SpanAudioCodec.Match(slice);
                if (a != null) { result.AudioCodec = a; t.Matched = true; continue; }
            }

            // Audio channels (two-token: "5" + "1")
            if (result.AudioChannels == null)
            {
                var ch = MatchChannels(slice);
                if (ch != null) { result.AudioChannels = ch; t.Matched = true; continue; }
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var ch2 = MatchChannelsTwo(slice, tokens[i + 1].Slice(name));
                    if (ch2 != null) { result.AudioChannels = ch2; t.Matched = true; tokens[i + 1].Matched = true; continue; }
                }
            }

            // Screen size — returns canonical string (no ToLowerInvariant alloc)
            if (result.ScreenSize == null)
            {
                var ss = SpanScreenSize.Match(slice);
                if (ss != null)
                {
                    result.ScreenSize = ss;
                    t.Matched = true; continue;
                }
            }

            // Source (two-token lookahead)
            if (result.Source == null)
            {
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var src2 = SpanSource.MatchTwo(slice, tokens[i + 1].Slice(name));
                    if (src2 != null)
                    {
                        result.Source = src2;
                        t.Matched = true;
                        tokens[i + 1].Matched = true;
                        continue;
                    }
                }
                var s = SpanSource.Match(slice);
                if (s != null) { result.Source = s; t.Matched = true; continue; }
            }

            // Streaming service
            if (result.StreamingService == null)
            {
                var sv = SpanStreamingService.Match(slice);
                if (sv != null) { result.StreamingService = sv; t.Matched = true; continue; }
            }

            // Edition (two-token lookahead)
            if (i + 1 < tokenCount && !tokens[i + 1].Matched)
            {
                var ed2 = SpanEdition.MatchTwo(slice, tokens[i + 1].Slice(name));
                if (ed2 != null)
                {
                    result.Edition.Add(ed2);
                    t.Matched = true;
                    tokens[i + 1].Matched = true;
                    continue;
                }
            }
            var ed = SpanEdition.Match(slice);
            if (ed != null) { result.Edition.Add(ed); t.Matched = true; continue; }

            // Other (two-token lookahead)
            if (i + 1 < tokenCount && !tokens[i + 1].Matched)
            {
                var ot2 = SpanOther.MatchTwo(slice, tokens[i + 1].Slice(name));
                if (ot2 != null)
                {
                    result.Other.Add(ot2);
                    t.Matched = true;
                    tokens[i + 1].Matched = true;
                    continue;
                }
            }
            var ot = SpanOther.Match(slice);
            if (ot != null) { result.Other.Add(ot); t.Matched = true; continue; }

            // Color depth
            if (result.ColorDepth == null)
            {
                var cd = SpanColorDepth.Match(slice);
                if (cd != null) { result.ColorDepth = cd; t.Matched = true; continue; }
            }

            // Language
            if (result.Language == null)
            {
                var lang = SpanDictLookup.Lookup(slice, LanguageMap);
                if (lang != null) { result.Language = lang; t.Matched = true; continue; }
            }

            // Year
            if (result.Year == null && IsYear(slice, out var year))
            {
                result.Year = year;
                t.Matched = true;
                continue;
            }
        }

        // Absolute episode (if no S##E## found)
        // ponytail: Only consider tokens after index 1 to avoid claiming title numbers
        // (e.g. "The 100" → "100" is the title, not episode 100). A true absolute episode
        // usually appears after at least a title token or bracket group.
        if (result is { HasSeason: false, HasEpisode: false })
        {
            for (var i = 0; i < tokenCount; i++)
            {
                ref var t = ref tokens[i];
                if (t.Matched || t.IsBracket) continue;
                var slice = t.Slice(name);
                if (!IsAbsoluteEpisode(slice, out var ep)) continue;
                // Skip if this token is adjacent to leading unmatched tokens (likely title)
                // Only claim it if there's at least one unmatched token before it (title) AND
                // at least one matched token after it (quality/codec info confirming it's structured)
                var hasLeadingTitle = false;
                for (var j = 0; j < i; j++)
                {
                    if ((tokens[j].Matched || tokens[j].IsBracket) && !tokens[j].IsBracket) continue;
                    hasLeadingTitle = true; break;
                }
                var hasTrailingMatched = false;
                for (var j = i + 1; j < tokenCount; j++)
                {
                    if (!tokens[j].Matched) continue;
                    hasTrailingMatched = true; break;
                }

                if (!hasLeadingTitle || !hasTrailingMatched) continue;
                result.AbsoluteEpisode.Add(ep);
                t.Matched = true;
                break;
            }
        }

        // Release group: first unmatched bracket group, or last token after last matched
        ExtractReleaseGroup(name, tokens, tokenCount, result);

        // Title: leading unmatched non-bracket tokens
        ExtractTitle(name, tokens, tokenCount, result);

        // Determine type
        result.Type = (result.HasSeason || result.HasEpisode || result.HasAbsoluteEpisode)
            ? MediaType.Episode : MediaType.Movie;

        return result;
    }

    /// <summary>
    /// Zero-allocation variant: guess media properties from a filename string.
    /// Returns a ref struct with span slices into the input — no heap strings for title/group.
    /// </summary>
    public static ZeroAllocGuessResult GuessZeroAlloc(string filename)
    {
        return string.IsNullOrWhiteSpace(filename) ? throw new ArgumentException("Filename cannot be null or empty.", nameof(filename)) : GuessZeroAlloc(filename.AsSpan());
    }

    /// <summary>
    /// Zero-allocation variant: guess media properties from a filename span.
    /// Title and ReleaseGroup are returned as raw span slices (no dot-to-space replacement).
    /// </summary>
    public static ZeroAllocGuessResult GuessZeroAlloc(ReadOnlySpan<char> input)
    {
        var result = new ZeroAllocGuessResult();

        // Extract extension
        var ext = SpanTokenizer.GetExtension(input);
        if (!ext.IsEmpty)
        {
            var container = SpanContainer.TryGetCanonical(ext);
            if (container != null)
            {
                result.Container = container;
                result.Mimetype = SpanContainer.GetMime(ext);
            }
        }

        // Get the name without path/extension
        var name = SpanTokenizer.GetFileNameWithoutExtension(input);

        // Tokenize onto the stack
        Span<SpanToken> tokens = stackalloc SpanToken[SpanTokenizer.MaxTokens];
        var tokenCount = SpanTokenizer.TokenizeName(name, tokens);

        // Episode matching
        MatchEpisodesZeroAlloc(name, tokens, tokenCount, ref result);

        // Single-token property matching
        for (var i = 0; i < tokenCount; i++)
        {
            ref var t = ref tokens[i];
            if (t.Matched) continue;

            var slice = t.Slice(name);

            if (result.VideoCodec == null)
            {
                var v = SpanVideoCodec.Match(slice);
                if (v != null) { result.VideoCodec = v; t.Matched = true; continue; }
            }

            if (result.AudioCodec == null)
            {
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var next = tokens[i + 1].Slice(name);
                    var twoMatch = SpanAudioCodec.MatchTwo(slice, next);
                    if (twoMatch != null)
                    {
                        result.AudioCodec = twoMatch;
                        t.Matched = true;
                        tokens[i + 1].Matched = true;
                        continue;
                    }
                }
                var a = SpanAudioCodec.Match(slice);
                if (a != null) { result.AudioCodec = a; t.Matched = true; continue; }
            }

            if (result.AudioChannels == null)
            {
                var ch = MatchChannels(slice);
                if (ch != null) { result.AudioChannels = ch; t.Matched = true; continue; }
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var ch2 = MatchChannelsTwo(slice, tokens[i + 1].Slice(name));
                    if (ch2 != null) { result.AudioChannels = ch2; t.Matched = true; tokens[i + 1].Matched = true; continue; }
                }
            }

            if (result.ScreenSize == null)
            {
                var ss = SpanScreenSize.Match(slice);
                if (ss != null) { result.ScreenSize = ss; t.Matched = true; continue; }
            }

            if (result.Source == null)
            {
                var s = SpanSource.Match(slice);
                if (s != null) { result.Source = s; t.Matched = true; continue; }
            }

            if (result.StreamingService == null)
            {
                var sv = SpanStreamingService.Match(slice);
                if (sv != null) { result.StreamingService = sv; t.Matched = true; continue; }
            }

            // Edition
            var ed = SpanEdition.Match(slice);
            if (ed != null && result.EditionCount < 4)
            {
                result.Edition[result.EditionCount++] = ed;
                t.Matched = true; continue;
            }

            // Other
            var ot = SpanOther.Match(slice);
            if (ot != null && result.OtherCount < 4)
            {
                result.Other[result.OtherCount++] = ot;
                t.Matched = true; continue;
            }

            if (result.ColorDepth == null)
            {
                var cd = SpanColorDepth.Match(slice);
                if (cd != null) { result.ColorDepth = cd; t.Matched = true; continue; }
            }

            // Language
            if (result.Language == null)
            {
                var lang = SpanDictLookup.Lookup(slice, LanguageMap);
                if (lang != null) { result.Language = lang; t.Matched = true; continue; }
            }

            if (result.Year != null || !IsYear(slice, out var year)) continue;
            result.Year = year;
            t.Matched = true;
            continue;
        }

        // Absolute episode (if no S##E## found)
        if (result.SeasonCount == 0 && result.EpisodeCount == 0)
        {
            for (var i = 0; i < tokenCount; i++)
            {
                ref var t = ref tokens[i];
                if (t.Matched || t.IsBracket) continue;
                var slice = t.Slice(name);
                if (!IsAbsoluteEpisode(slice, out var ep)) continue;
                if (result.AbsoluteEpisodeCount < 4)
                    result.AbsoluteEpisode[result.AbsoluteEpisodeCount++] = ep;
                t.Matched = true;
                break;
            }
        }

        // Release group: span slice into name (zero-alloc)
        ExtractReleaseGroupZeroAlloc(name, tokens, tokenCount, ref result);

        // Title: raw span slice into name (dots NOT replaced — see comment below)
        ExtractTitleZeroAlloc(name, tokens, tokenCount, ref result);

        // Determine type
        result.Type = (result.SeasonCount > 0 || result.EpisodeCount > 0 || result.AbsoluteEpisodeCount > 0)
            ? MediaType.Episode : MediaType.Movie;

        return result;
    }

    private static void MatchEpisodes(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        for (var i = 0; i < count; i++)
        {
            if (tokens[i].Matched || tokens[i].IsBracket) continue;
            var slice = tokens[i].Slice(name);

            // Check S##E##
            if (slice.Length >= 4 && (slice[0] == 'S' || slice[0] == 's'))
            {
                if (TryParseSxE(slice, result))
                {
                    tokens[i].Matched = true;
                    continue;
                }
            }

            // Check NxNN
            var xPos = slice.IndexOf('x');
            if (xPos < 0) xPos = slice.IndexOf('X');
            if (xPos <= 0 || xPos >= slice.Length - 1) continue;
            if (TryParseCross(slice, xPos, result))
            {
                tokens[i].Matched = true;
            }
        }
    }

    private static bool TryParseSxE(ReadOnlySpan<char> slice, SpanGuessResult result)
    {
        var i = 1; // skip 'S'
        var season = 0;
        while (i < slice.Length && char.IsDigit(slice[i]))
        {
            season = season * 10 + (slice[i] - '0');
            i++;
        }
        if (season == 0 || i >= slice.Length) return false;
        if (slice[i] != 'E' && slice[i] != 'e') return false;

        result.Season.Add(season);

        while (i < slice.Length && (slice[i] == 'E' || slice[i] == 'e'))
        {
            i++; // skip 'E'
            var ep = 0;
            while (i < slice.Length && char.IsDigit(slice[i]))
            {
                ep = ep * 10 + (slice[i] - '0');
                i++;
            }
            if (ep > 0) result.Episode.Add(ep);
            while (i < slice.Length && (slice[i] == '-' || slice[i] == ' ')) i++;
        }

        return result.HasEpisode;
    }

    private static bool TryParseCross(ReadOnlySpan<char> slice, int xPos, SpanGuessResult result)
    {
        var season = 0;
        for (var i = 0; i < xPos; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            season = season * 10 + (slice[i] - '0');
        }
        var episode = 0;
        for (var i = xPos + 1; i < slice.Length; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            episode = episode * 10 + (slice[i] - '0');
        }

        if (season <= 0 || episode <= 0) return false;
        result.Season.Add(season);
        result.Episode.Add(episode);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsYear(ReadOnlySpan<char> slice, out int year)
    {
        year = 0;
        if (slice.Length != 4) return false;
        foreach (var c in slice)
        {
            if (!char.IsDigit(c)) return false;
            year = year * 10 + (c - '0');
        }
        // ponytail: ceiling bumped to 2035. Revisit when media from 2036 exists.
        return year >= 1920 && year <= 2035;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAbsoluteEpisode(ReadOnlySpan<char> slice, out int ep)
    {
        ep = 0;
        if (slice.Length is < 2 or > 4) return false;
        foreach (var c in slice)
        {
            if (!char.IsDigit(c)) { ep = 0; return false; }
            ep = ep * 10 + (c - '0');
        }
        return ep >= 1 && ep <= 1999 && ep is < 1920 or > 2035;
    }

    private static string? MatchChannels(ReadOnlySpan<char> slice)
    {
        if (slice.Equals("7.1", StringComparison.Ordinal)) return "7.1";
        if (slice.Equals("5.1", StringComparison.Ordinal)) return "5.1";
        if (slice.Equals("2.0", StringComparison.Ordinal)) return "2.0";
        if (slice.Equals("1.0", StringComparison.Ordinal)) return "1.0";
        if (slice.Equals("stereo", StringComparison.OrdinalIgnoreCase)) return "2.0";
        if (slice.Equals("mono", StringComparison.OrdinalIgnoreCase)) return "1.0";
        return null;
    }

    private static string? MatchChannelsTwo(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        if (first.Length != 1 || second.Length != 1) return null;
        if (first[0] == '7' && second[0] == '1') return "7.1";
        if (first[0] == '5' && second[0] == '1') return "5.1";
        if (first[0] == '2' && second[0] == '0') return "2.0";
        if (first[0] == '1' && second[0] == '0') return "1.0";
        return null;
    }

    private static void ExtractReleaseGroup(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        // Strategy 1: first unmatched bracket group
        for (var i = 0; i < count; i++)
        {
            if (!tokens[i].IsBracket || tokens[i].Matched) continue;
            var slice = tokens[i].Slice(name);
            if (!IsGroupName(slice)) continue;
            result.ReleaseGroup = new string(slice);
            tokens[i].Matched = true;
            return;
        }

        // Strategy 2: last unmatched non-bracket token after last matched
        var lastMatched = -1;
        for (var i = 0; i < count; i++)
            if (tokens[i].Matched && !tokens[i].IsBracket) lastMatched = i;

        if (lastMatched < 0) return;

        for (var i = count - 1; i > lastMatched; i--)
        {
            if (tokens[i].Matched || tokens[i].IsBracket) continue;
            var slice = tokens[i].Slice(name);
            if (IsGroupName(slice))
            {
                result.ReleaseGroup = new string(slice);
                tokens[i].Matched = true;
            }
            break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsGroupName(ReadOnlySpan<char> slice)
    {
        if (slice.Length is < 2 or > 26) return false;
        if (!char.IsLetter(slice[0])) return false;
        for (var i = 1; i < slice.Length; i++)
        {
            var c = slice[i];
            if (!char.IsLetterOrDigit(c) && c != '-') return false;
        }
        return true;
    }

    private static void ExtractTitle(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        var start = 0;
        while (start < count && tokens[start].IsBracket) start++;

        var titleEnd = start;
        while (titleEnd < count && !tokens[titleEnd].Matched && !tokens[titleEnd].IsBracket)
            titleEnd++;

        if (titleEnd > start)
        {
            var totalLen = 0;
            for (var i = start; i < titleEnd; i++)
            {
                if (i > start) totalLen++;
                totalLen += tokens[i].Length;
                tokens[i].Matched = true;
            }

            // ponytail: One allocation here for the final title string. Can't avoid it — caller needs a string.
            var buffer = totalLen <= 128 ? stackalloc char[totalLen] : new char[totalLen];
            var pos = 0;
            for (var i = start; i < titleEnd; i++)
            {
                if (i > start) buffer[pos++] = ' ';
                tokens[i].Slice(name).CopyTo(buffer[pos..]);
                pos += tokens[i].Length;
            }
            result.Title = new string(buffer);
        }

        // Episode title: unmatched tokens immediately after the episode marker
        // e.g. "S01E01.Pilot.720p" → EpisodeTitle = "Pilot"
        if (result.HasSeason || result.HasEpisode)
        {
            ExtractEpisodeTitle(name, tokens, count, result);
        }
    }

    private static void ExtractEpisodeTitle(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        // Find the episode token (the matched S##E## or NxNN token)
        var epIndex = -1;
        for (var i = 0; i < count; i++)
        {
            if (!tokens[i].Matched || tokens[i].IsBracket) continue;
            var slice = tokens[i].Slice(name);
            if (slice.Length >= 4 && (slice[0] == 'S' || slice[0] == 's') && char.IsDigit(slice[1]))
            {
                epIndex = i;
                break;
            }
            // NxNN
            var xp = slice.IndexOf('x');
            if (xp < 0) xp = slice.IndexOf('X');
            if (xp <= 0 || xp >= slice.Length - 1 || !char.IsDigit(slice[0])) continue;
            epIndex = i;
            break;
        }

        if (epIndex < 0) return;

        // Collect consecutive unmatched non-bracket tokens after the episode marker
        var epTitleStart = epIndex + 1;
        while (epTitleStart < count && (tokens[epTitleStart].Matched || tokens[epTitleStart].IsBracket))
            epTitleStart++;

        var epTitleEnd = epTitleStart;
        while (epTitleEnd < count && !tokens[epTitleEnd].Matched && !tokens[epTitleEnd].IsBracket)
            epTitleEnd++;

        if (epTitleEnd > epTitleStart)
        {
            var totalLen = 0;
            for (var i = epTitleStart; i < epTitleEnd; i++)
            {
                if (i > epTitleStart) totalLen++;
                totalLen += tokens[i].Length;
                tokens[i].Matched = true;
            }

            var buffer = totalLen <= 64 ? stackalloc char[totalLen] : new char[totalLen];
            var pos = 0;
            for (var i = epTitleStart; i < epTitleEnd; i++)
            {
                if (i > epTitleStart) buffer[pos++] = ' ';
                tokens[i].Slice(name).CopyTo(buffer[pos..]);
                pos += tokens[i].Length;
            }
            result.EpisodeTitle = new string(buffer);
        }
    }

    // --- ZeroAlloc helpers ---

    private static void MatchEpisodesZeroAlloc(ReadOnlySpan<char> name, scoped Span<SpanToken> tokens, int count, ref ZeroAllocGuessResult result)
    {
        for (var i = 0; i < count; i++)
        {
            if (tokens[i].Matched || tokens[i].IsBracket) continue;
            var slice = tokens[i].Slice(name);

            if (slice.Length >= 4 && (slice[0] == 'S' || slice[0] == 's'))
            {
                if (TryParseSxEZeroAlloc(slice, ref result))
                {
                    tokens[i].Matched = true;
                    continue;
                }
            }

            var xPos = slice.IndexOf('x');
            if (xPos < 0) xPos = slice.IndexOf('X');
            if (xPos <= 0 || xPos >= slice.Length - 1) continue;
            if (TryParseCrossZeroAlloc(slice, xPos, ref result))
            {
                tokens[i].Matched = true;
            }
        }
    }

    private static bool TryParseSxEZeroAlloc(ReadOnlySpan<char> slice, ref ZeroAllocGuessResult result)
    {
        var i = 1;
        var season = 0;
        while (i < slice.Length && char.IsDigit(slice[i]))
        {
            season = season * 10 + (slice[i] - '0');
            i++;
        }
        if (season == 0 || i >= slice.Length) return false;
        if (slice[i] != 'E' && slice[i] != 'e') return false;

        if (result.SeasonCount < 4)
            result.Season[result.SeasonCount++] = season;

        var hasEp = false;
        while (i < slice.Length && (slice[i] == 'E' || slice[i] == 'e'))
        {
            i++;
            var ep = 0;
            while (i < slice.Length && char.IsDigit(slice[i]))
            {
                ep = ep * 10 + (slice[i] - '0');
                i++;
            }
            if (ep > 0 && result.EpisodeCount < 4)
            {
                result.Episode[result.EpisodeCount++] = ep;
                hasEp = true;
            }
            while (i < slice.Length && (slice[i] == '-' || slice[i] == ' ')) i++;
        }

        return hasEp;
    }

    private static bool TryParseCrossZeroAlloc(ReadOnlySpan<char> slice, int xPos, ref ZeroAllocGuessResult result)
    {
        var season = 0;
        for (var i = 0; i < xPos; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            season = season * 10 + (slice[i] - '0');
        }
        var episode = 0;
        for (var i = xPos + 1; i < slice.Length; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            episode = episode * 10 + (slice[i] - '0');
        }

        if (season <= 0 || episode <= 0) return false;
        if (result.SeasonCount < 4) result.Season[result.SeasonCount++] = season;
        if (result.EpisodeCount < 4) result.Episode[result.EpisodeCount++] = episode;
        return true;
    }

    private static void ExtractReleaseGroupZeroAlloc(ReadOnlySpan<char> name, scoped Span<SpanToken> tokens, int count, ref ZeroAllocGuessResult result)
    {
        // Strategy 1: first unmatched bracket group
        for (var i = 0; i < count; i++)
        {
            if (!tokens[i].IsBracket || tokens[i].Matched) continue;
            var slice = tokens[i].Slice(name);
            if (IsGroupName(slice))
            {
                result.ReleaseGroup = slice;
                tokens[i].Matched = true;
                return;
            }
        }

        // Strategy 2: last unmatched non-bracket token after last matched
        var lastMatched = -1;
        for (var i = 0; i < count; i++)
            if (tokens[i].Matched && !tokens[i].IsBracket) lastMatched = i;

        if (lastMatched < 0) return;

        for (var i = count - 1; i > lastMatched; i--)
        {
            if (tokens[i].Matched || tokens[i].IsBracket) continue;
            var slice = tokens[i].Slice(name);
            if (IsGroupName(slice))
            {
                result.ReleaseGroup = slice;
                tokens[i].Matched = true;
            }
            break;
        }
    }

    private static void ExtractTitleZeroAlloc(ReadOnlySpan<char> name, scoped Span<SpanToken> tokens, int count, ref ZeroAllocGuessResult result)
    {
        var start = 0;
        while (start < count && tokens[start].IsBracket) start++;

        var titleEnd = start;
        while (titleEnd < count && !tokens[titleEnd].Matched && !tokens[titleEnd].IsBracket)
            titleEnd++;

        if (titleEnd <= start) return;
        // note: Zero-alloc path stores the RAW title span from name — dots/separators between
        // tokens are NOT replaced with spaces. The caller gets the original characters including
        // separators (e.g. "The.Matrix" not "The Matrix"). This avoids the allocation needed for
        // dot-to-space replacement. Call .ToString() and replace manually if display form is needed.
        var spanStart = tokens[start].Start;
        var spanEnd = tokens[titleEnd - 1].Start + tokens[titleEnd - 1].Length;
        result.Title = name[spanStart..spanEnd];

        for (var i = start; i < titleEnd; i++)
            tokens[i].Matched = true;
    }
}
