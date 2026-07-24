using System;
using System.Runtime.CompilerServices;

namespace Terka.Span;

/// <summary>
/// A token represented as a range into the original string. Avoids allocation during tokenization.
/// </summary>
internal struct SpanToken
{
    public int Start;
    public int Length;
    public bool IsBracket;
    public bool Matched;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => source.Slice(Start, Length);
}

/// <summary>
/// Zero-allocation tokenizer using ReadOnlySpan&lt;char&gt;.
/// Splits on dots, spaces, underscores, dashes while tracking bracket groups.
/// Tokens are stored as index ranges into the original string.
/// </summary>
internal static class SpanTokenizer
{
    // ponytail: Fixed-size token buffer. 32 tokens is plenty for any realistic filename.
    // Upgrade path: stackalloc a larger buffer or fall back to ArrayPool if exceeded.
    public const int MaxTokens = 32;

    /// <summary>
    /// Tokenize the filename (no path, no extension) in-place.
    /// Caller should stackalloc SpanToken[MaxTokens].
    /// Returns the number of tokens written to the buffer.
    /// </summary>
    public static int TokenizeName(ReadOnlySpan<char> name, Span<SpanToken> tokens)
    {
        var count = 0;
        var i = 0;

        while (i < name.Length && count < MaxTokens)
        {
            var c = name[i];

            if (c is '[' or '(')
            {
                var close = c == '[' ? ']' : ')';
                var start = i + 1;
                var end = start;
                while (end < name.Length && name[end] != close) end++;
                if (end > start)
                {
                    tokens[count++] = new SpanToken { Start = start, Length = end - start, IsBracket = true };
                }
                i = (end < name.Length) ? end + 1 : end;
                continue;
            }

            if (IsSeparator(c))
            {
                i++;
                continue;
            }

            var tokenStart = i;
            while (i < name.Length && !IsSeparator(name[i]) && name[i] != '[' && name[i] != '(' && name[i] != ']' && name[i] != ')')
                i++;

            tokens[count++] = new SpanToken { Start = tokenStart, Length = i - tokenStart, IsBracket = false };
        }

        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSeparator(char c) => c is '.' or ' ' or '_' or '-';

    /// <summary>
    /// Extract extension from a full filename/path. Returns empty if none.
    /// </summary>
    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> input)
    {
        var lastDot = input.LastIndexOf('.');
        return lastDot <= 0 ? ReadOnlySpan<char>.Empty : input[(lastDot + 1)..];
    }

    /// <summary>
    /// Strip path and extension, returning just the name portion.
    /// </summary>
    public static ReadOnlySpan<char> GetFileNameWithoutExtension(ReadOnlySpan<char> input)
    {
        var lastSlash = input.LastIndexOfAny('/', '\\');
        if (lastSlash >= 0) input = input[(lastSlash + 1)..];
        var lastDot = input.LastIndexOf('.');
        if (lastDot > 0) input = input[..lastDot];
        return input;
    }
}
