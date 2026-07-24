using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Terka
{
    /// <summary>
    /// Breaks a filename into tokens by splitting on common separators
    /// (dots, underscores, spaces, dashes, brackets) while preserving bracket groups.
    /// </summary>
    internal static class Tokenizer
    {
        // ponytail: Naive O(n) single-pass tokenizer. Good enough for media filenames.
        // Upgrade path: multi-pass rebulk-style if rule conflicts arise.

        private static readonly Regex BracketGroup = new Regex(
            @"[\[\(]([^\]\)]+)[\]\)]", RegexOptions.Compiled);

        private static readonly Regex SplitPattern = new Regex(
            @"[\.\s_\-]+", RegexOptions.Compiled);

        /// <summary>
        /// Tokenize a raw input (filename or path) into ordered tokens.
        /// Strips path and extension first.
        /// </summary>
        public static TokenResult Tokenize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new TokenResult(input, []);

            // Strip path, keep filename only
            var name = Path.GetFileNameWithoutExtension(input);
            var ext = Path.GetExtension(input)?.TrimStart('.');

            // Extract bracket groups first (e.g., [720p], (HEVC))
            var bracketTokens = new List<Token>();
            var cleaned = BracketGroup.Replace(name, m =>
            {
                bracketTokens.Add(new Token(m.Groups[1].Value.Trim(), m.Index, isBracketGroup: true));
                return " "; // replace with space so position-based title extraction still works
            });

            // Split remaining on separators
            var parts = SplitPattern.Split(cleaned);
            var tokens = new List<Token>();
            int pos = 0;
            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    tokens.Add(new Token(part, pos, isBracketGroup: false));
                }
                pos++;
            }

            // Append bracket tokens at the end (they'll be scanned by matchers)
            tokens.AddRange(bracketTokens);

            return new TokenResult(name, tokens.ToArray(), ext);
        }
    }

    internal class TokenResult
    {
        public string OriginalName { get; }
        public Token[] Tokens { get; }
        public string Extension { get; }

        public TokenResult(string originalName, Token[] tokens, string extension = null)
        {
            OriginalName = originalName;
            Tokens = tokens;
            Extension = extension;
        }
    }

    internal class Token
    {
        public string Value { get; }
        public int Position { get; }
        public bool IsBracketGroup { get; }
        /// <summary>Marks this token as consumed by a matcher so title extraction skips it.</summary>
        public bool Matched { get; set; }

        public Token(string value, int position, bool isBracketGroup)
        {
            Value = value;
            Position = position;
            IsBracketGroup = isBracketGroup;
        }

        public override string ToString() => Value;
    }
}
