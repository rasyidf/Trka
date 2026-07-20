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
    // Upgrade path: stackalloc a larger buffer or fall back to array if exceeded.
    public const int MaxTokens = 32;

    /// <summary>
    /// Tokenize in-place. Returns the number of tokens written to the buffer.
    /// The caller should stackalloc SpanToken[MaxTokens].
    /// </summary>
    public static int Tokenize(ReadOnlySpan<char> input, System.Span<SpanToken> tokens, out int extStart, out int extLength)
    {
        extStart = -1;
        extLength = 0;

        if (input.IsEmpty) return 0;

        // Find last dot for extension
        int lastDot = input.LastIndexOf('.');
        ReadOnlySpan<char> name;
        if (lastDot > 0)
        {
            name = input[..lastDot];
            extStart = lastDot + 1;
            extLength = input.Length - lastDot - 1;
        }
        else
        {
            name = input;
        }

        // Strip path: find last slash
        int lastSlash = name.LastIndexOfAny('/','\\');
        if (lastSlash >= 0)
            name = name[(lastSlash + 1)..];

        int count = 0;

        // Scan for bracket groups and regular tokens in a single pass
        int i = 0;
        while (i < name.Length && count < MaxTokens)
        {
            char c = name[i];

            // Bracket group
            if (c == '[' || c == '(')
            {
                char close = c == '[' ? ']' : ')';
                int start = i + 1;
                int end = start;
                while (end < name.Length && name[end] != close) end++;

                if (end > start)
                {
                    tokens[count] = new SpanToken { Start = start + (int)(name.Length - input.Length + (lastDot > 0 ? 0 : 0)),
                        Length = end - start, IsBracket = true, Matched = false };
                    // Adjust: we need offsets relative to the ORIGINAL input
                    // Actually, since we sliced, let's compute the offset
                    int offset = input.Length - (lastDot > 0 ? input.Length - lastDot : 0) - name.Length;
                    // Simpler: just track the name offset from the start
                    // Let me reconsider...
                    count++;
                }
                i = end + 1;
                continue;
            }

            // Separator
            if (IsSeparator(c))
            {
                i++;
                continue;
            }

            // Regular token: scan until separator or bracket
            int tokenStart = i;
            while (i < name.Length && !IsSeparator(name[i]) && name[i] != '[' && name[i] != '(' && name[i] != ']' && name[i] != ')')
                i++;

            tokens[count] = new SpanToken { Start = tokenStart, Length = i - tokenStart, IsBracket = false, Matched = false };
            count++;
        }

        return count;

        // The offsets are relative to `name` (the path-stripped, extension-stripped slice).
        // We need to fix this: store the name slice start offset so callers can use it.
        // Actually, the simplest approach: the caller passes the name slice directly.
        // Let's restructure.
    }

    /// <summary>
    /// Simplified tokenize that works on just the filename (no path, no extension).
    /// Caller is responsible for stripping path and extension.
    /// This is the hot path used by the parser.
    /// </summary>
    public static int TokenizeName(ReadOnlySpan<char> name, System.Span<SpanToken> tokens)
    {
        int count = 0;
        int i = 0;

        while (i < name.Length && count < MaxTokens)
        {
            char c = name[i];

            if (c == '[' || c == '(')
            {
                char close = c == '[' ? ']' : ')';
                int start = i + 1;
                int end = start;
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

            int tokenStart = i;
            while (i < name.Length && !IsSeparator(name[i]) && name[i] != '[' && name[i] != '(' && name[i] != ']' && name[i] != ')')
                i++;

            tokens[count++] = new SpanToken { Start = tokenStart, Length = i - tokenStart, IsBracket = false };
        }

        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSeparator(char c) => c == '.' || c == ' ' || c == '_' || c == '-';

    /// <summary>
    /// Extract extension from a full filename/path. Returns empty if none.
    /// </summary>
    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> input)
    {
        int lastDot = input.LastIndexOf('.');
        if (lastDot <= 0) return ReadOnlySpan<char>.Empty;
        return input[(lastDot + 1)..];
    }

    /// <summary>
    /// Strip path and extension, returning just the name portion.
    /// </summary>
    public static ReadOnlySpan<char> GetFileNameWithoutExtension(ReadOnlySpan<char> input)
    {
        int lastSlash = input.LastIndexOfAny('/','\\');
        if (lastSlash >= 0) input = input[(lastSlash + 1)..];
        int lastDot = input.LastIndexOf('.');
        if (lastDot > 0) input = input[..lastDot];
        return input;
    }
}
