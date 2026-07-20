using System;
using System.Collections.Generic;

namespace Terka.Matchers
{
    /// <summary>
    /// Audio codec matcher that also handles multi-token patterns (e.g. "DTS" + "HD" → "DTS-HD").
    /// </summary>
    internal class AudioCodecMatcher : IMatcher
    {
        private static readonly Dictionary<string, string> SingleTokenMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["aac"] = "AAC",
            ["ac3"] = "Dolby Digital",
            ["dd"] = "Dolby Digital",
            ["dd5.1"] = "Dolby Digital",
            ["eac3"] = "Dolby Digital Plus",
            ["ddp"] = "Dolby Digital Plus",
            ["ddp5.1"] = "Dolby Digital Plus",
            ["truehd"] = "Dolby TrueHD",
            ["atmos"] = "Dolby Atmos",
            ["dts"] = "DTS",
            ["dtshd"] = "DTS-HD",
            ["dts-hd"] = "DTS-HD",
            ["dts-hdma"] = "DTS-HD",
            ["dtsx"] = "DTS:X",
            ["dts-x"] = "DTS:X",
            ["flac"] = "FLAC",
            ["lpcm"] = "LPCM",
            ["pcm"] = "PCM",
            ["mp2"] = "MP2",
            ["mp3"] = "MP3",
            ["opus"] = "Opus",
            ["vorbis"] = "Vorbis",
        };

        // Two-token combinations: (first, second) → canonical
        private static readonly (string First, string Second, string Canonical)[] TwoTokenCombos =
        {
            ("dts", "hd", "DTS-HD"),
            ("dts", "hdma", "DTS-HD"),
            ("dts", "x", "DTS:X"),
            ("true", "hd", "Dolby TrueHD"),
        };

        public void Match(Token[] tokens, GuessResult result)
        {
            // First pass: look for two-token combos
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (tokens[i].Matched) continue;
                var next = FindNextUnmatched(tokens, i + 1);
                if (next < 0) break;

                foreach (var combo in TwoTokenCombos)
                {
                    if (string.Equals(tokens[i].Value, combo.First, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(tokens[next].Value, combo.Second, StringComparison.OrdinalIgnoreCase))
                    {
                        result.AudioCodec = combo.Canonical;
                        tokens[i].Matched = true;
                        tokens[next].Matched = true;
                        return;
                    }
                }
            }

            // Second pass: single-token matches
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                if (SingleTokenMap.TryGetValue(token.Value, out var canonical))
                {
                    result.AudioCodec = canonical;
                    token.Matched = true;
                    return;
                }
            }
        }

        private static int FindNextUnmatched(Token[] tokens, int start)
        {
            for (int i = start; i < tokens.Length; i++)
                if (!tokens[i].Matched) return i;
            return -1;
        }
    }
}
