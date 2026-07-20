using System;
using System.Runtime.CompilerServices;

namespace Terka.Span;

/// <summary>
/// High-performance span-based media filename parser.
/// Minimizes allocations by operating on ReadOnlySpan&lt;char&gt; slices.
/// </summary>
public static class SpanGuessIt
{
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
    /// Guess media properties from a filename span.
    /// </summary>
    public static SpanGuessResult Guess(ReadOnlySpan<char> input)
    {
        var result = new SpanGuessResult();

        // Extract extension
        var ext = SpanTokenizer.GetExtension(input);
        if (!ext.IsEmpty && SpanContainer.IsKnown(ext))
        {
            result.Container = new string(ext).ToLowerInvariant();
            result.Mimetype = SpanContainer.GetMime(ext);
        }

        // Get the name without path/extension
        var name = SpanTokenizer.GetFileNameWithoutExtension(input);

        // Tokenize onto the stack
        Span<SpanToken> tokens = stackalloc SpanToken[SpanTokenizer.MaxTokens];
        int tokenCount = SpanTokenizer.TokenizeName(name, tokens);

        // Episode matching (S##E## across joined tokens)
        MatchEpisodes(name, tokens, tokenCount, result);

        // Single-token property matching
        for (int i = 0; i < tokenCount; i++)
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
                // Try two-token combo first
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
                // Two-token channel: "5"+"1", "7"+"1"
                if (i + 1 < tokenCount && !tokens[i + 1].Matched)
                {
                    var ch2 = MatchChannelsTwo(slice, tokens[i + 1].Slice(name));
                    if (ch2 != null) { result.AudioChannels = ch2; t.Matched = true; tokens[i + 1].Matched = true; continue; }
                }
            }

            // Screen size
            if (result.ScreenSize == null)
            {
                if (IsScreenSize(slice))
                {
                    result.ScreenSize = new string(slice).ToLowerInvariant();
                    t.Matched = true; continue;
                }
            }

            // Source
            if (result.Source == null)
            {
                var s = SpanSource.Match(slice);
                if (s != null) { result.Source = s; t.Matched = true; continue; }
            }

            // Streaming service
            if (result.StreamingService == null)
            {
                var ss = SpanStreamingService.Match(slice);
                if (ss != null) { result.StreamingService = ss; t.Matched = true; continue; }
            }

            // Edition
            var ed = SpanEdition.Match(slice);
            if (ed != null) { result.Edition.Add(ed); t.Matched = true; continue; }

            // Other
            var ot = SpanOther.Match(slice);
            if (ot != null) { result.Other.Add(ot); t.Matched = true; continue; }

            // Color depth
            if (result.ColorDepth == null)
            {
                var cd = SpanColorDepth.Match(slice);
                if (cd != null) { result.ColorDepth = cd; t.Matched = true; continue; }
            }

            // Year
            if (result.Year == null && IsYear(slice, out int year))
            {
                result.Year = year;
                t.Matched = true;
                continue;
            }
        }

        // Absolute episode (if no S##E## found)
        if (result.Season.Count == 0 && result.Episode.Count == 0)
        {
            for (int i = 0; i < tokenCount; i++)
            {
                ref var t = ref tokens[i];
                if (t.Matched || t.IsBracket) continue;
                var slice = t.Slice(name);
                if (IsAbsoluteEpisode(slice, out int ep))
                {
                    result.AbsoluteEpisode.Add(ep);
                    t.Matched = true;
                    break;
                }
            }
        }

        // Release group: first unmatched bracket group, or last token after last matched
        ExtractReleaseGroup(name, tokens, tokenCount, result);

        // Title: leading unmatched non-bracket tokens
        ExtractTitle(name, tokens, tokenCount, result);

        // Determine type
        result.Type = (result.Season.Count > 0 || result.Episode.Count > 0 || result.AbsoluteEpisode.Count > 0)
            ? MediaType.Episode : MediaType.Movie;

        return result;
    }

    private static void MatchEpisodes(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        // Scan for S##E## pattern directly on token values
        for (int i = 0; i < count; i++)
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
            int xPos = slice.IndexOf('x');
            if (xPos < 0) xPos = slice.IndexOf('X');
            if (xPos > 0 && xPos < slice.Length - 1)
            {
                if (TryParseCross(slice, xPos, result))
                {
                    tokens[i].Matched = true;
                }
            }
        }
    }

    private static bool TryParseSxE(ReadOnlySpan<char> slice, SpanGuessResult result)
    {
        // S01E02, S01E02E03
        int i = 1; // skip 'S'
        int season = 0;
        while (i < slice.Length && char.IsDigit(slice[i]))
        {
            season = season * 10 + (slice[i] - '0');
            i++;
        }
        if (season == 0 || i >= slice.Length) return false;
        if (slice[i] != 'E' && slice[i] != 'e') return false;

        result.Season.Add(season);

        // Parse episodes
        while (i < slice.Length && (slice[i] == 'E' || slice[i] == 'e'))
        {
            i++; // skip 'E'
            int ep = 0;
            while (i < slice.Length && char.IsDigit(slice[i]))
            {
                ep = ep * 10 + (slice[i] - '0');
                i++;
            }
            if (ep > 0) result.Episode.Add(ep);
            // Skip separator between episodes
            while (i < slice.Length && (slice[i] == '-' || slice[i] == ' ')) i++;
        }

        return result.Episode.Count > 0;
    }

    private static bool TryParseCross(ReadOnlySpan<char> slice, int xPos, SpanGuessResult result)
    {
        // NxNN pattern
        int season = 0;
        for (int i = 0; i < xPos; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            season = season * 10 + (slice[i] - '0');
        }
        int episode = 0;
        for (int i = xPos + 1; i < slice.Length; i++)
        {
            if (!char.IsDigit(slice[i])) return false;
            episode = episode * 10 + (slice[i] - '0');
        }
        if (season > 0 && episode > 0)
        {
            result.Season.Add(season);
            result.Episode.Add(episode);
            return true;
        }
        return false;
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
        return year >= 1920 && year <= 2030;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAbsoluteEpisode(ReadOnlySpan<char> slice, out int ep)
    {
        ep = 0;
        if (slice.Length < 2 || slice.Length > 4) return false;
        foreach (var c in slice)
        {
            if (!char.IsDigit(c)) { ep = 0; return false; }
            ep = ep * 10 + (c - '0');
        }
        return ep >= 1 && ep <= 1999 && (ep < 1920 || ep > 2030);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsScreenSize(ReadOnlySpan<char> slice)
    {
        if (slice.Length < 4 || slice.Length > 9) return false;
        // Check for NNNNp or NNNNi
        char last = slice[^1];
        if (last == 'p' || last == 'i')
        {
            for (int i = 0; i < slice.Length - 1; i++)
                if (!char.IsDigit(slice[i])) return false;
            return true;
        }
        // Check for NNNNxNNNN
        int x = slice.IndexOf('x');
        if (x > 0 && x < slice.Length - 1)
        {
            for (int i = 0; i < x; i++)
                if (!char.IsDigit(slice[i])) return false;
            for (int i = x + 1; i < slice.Length; i++)
                if (!char.IsDigit(slice[i])) return false;
            return true;
        }
        return false;
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
        if (first.Length == 1 && second.Length == 1)
        {
            if (first[0] == '7' && second[0] == '1') return "7.1";
            if (first[0] == '5' && second[0] == '1') return "5.1";
            if (first[0] == '2' && second[0] == '0') return "2.0";
            if (first[0] == '1' && second[0] == '0') return "1.0";
        }
        return null;
    }

    private static void ExtractReleaseGroup(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        // Strategy 1: first unmatched bracket group
        for (int i = 0; i < count; i++)
        {
            if (!tokens[i].IsBracket || tokens[i].Matched) continue;
            var slice = tokens[i].Slice(name);
            if (IsGroupName(slice))
            {
                result.ReleaseGroup = new string(slice);
                tokens[i].Matched = true;
                return;
            }
        }

        // Strategy 2: last unmatched non-bracket token after last matched
        int lastMatched = -1;
        for (int i = 0; i < count; i++)
            if (tokens[i].Matched && !tokens[i].IsBracket) lastMatched = i;

        if (lastMatched < 0) return;

        for (int i = count - 1; i > lastMatched; i--)
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
        if (slice.Length < 2 || slice.Length > 26) return false;
        if (!char.IsLetter(slice[0])) return false;
        for (int i = 1; i < slice.Length; i++)
        {
            char c = slice[i];
            if (!char.IsLetterOrDigit(c) && c != '-') return false;
        }
        return true;
    }

    private static void ExtractTitle(ReadOnlySpan<char> name, Span<SpanToken> tokens, int count, SpanGuessResult result)
    {
        // Skip leading bracket tokens, collect consecutive unmatched non-bracket tokens
        int start = 0;
        while (start < count && tokens[start].IsBracket) start++;

        int titleEnd = start;
        while (titleEnd < count && !tokens[titleEnd].Matched && !tokens[titleEnd].IsBracket)
            titleEnd++;

        if (titleEnd > start)
        {
            // Build title from token slices
            // ponytail: One allocation here for the final title string. Could use ArrayPool but not worth it.
            int totalLen = 0;
            for (int i = start; i < titleEnd; i++)
            {
                if (i > start) totalLen++; // space
                totalLen += tokens[i].Length;
                tokens[i].Matched = true;
            }

            Span<char> buffer = totalLen <= 128 ? stackalloc char[totalLen] : new char[totalLen];
            int pos = 0;
            for (int i = start; i < titleEnd; i++)
            {
                if (i > start) buffer[pos++] = ' ';
                tokens[i].Slice(name).CopyTo(buffer[pos..]);
                pos += tokens[i].Length;
            }
            result.Title = new string(buffer);
        }
    }
}
