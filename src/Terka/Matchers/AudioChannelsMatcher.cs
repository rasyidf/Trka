using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects audio channel configuration (5.1, 7.1, 2.0, 1.0).
    /// Handles both single-token ("5.1") and two-token ("5" + "1") cases.
    /// </summary>
    internal class AudioChannelsMatcher : IMatcher
    {
        private static readonly Dictionary<string, string> SingleTokenMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["7.1"] = "7.1",
            ["5.1"] = "5.1",
            ["2.0"] = "2.0",
            ["1.0"] = "1.0",
            ["stereo"] = "2.0",
            ["mono"] = "1.0",
        };

        // Two-token combos: first + second → channels
        private static readonly (string First, string Second, string Channels)[] TwoTokenCombos =
        {
            ("7", "1", "7.1"),
            ("5", "1", "5.1"),
            ("2", "0", "2.0"),
            ("1", "0", "1.0"),
        };

        public void Match(Token[] tokens, GuessResult result)
        {
            // Single token first
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                if (SingleTokenMap.TryGetValue(token.Value, out var channels))
                {
                    result.AudioChannels = channels;
                    token.Matched = true;
                    return;
                }
            }

            // Two-token: "5" followed by "1"
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (tokens[i].Matched) continue;
                var next = i + 1;
                while (next < tokens.Length && tokens[next].Matched) next++;
                if (next >= tokens.Length) break;

                foreach (var combo in TwoTokenCombos)
                {
                    if (tokens[i].Value == combo.First && tokens[next].Value == combo.Second)
                    {
                        result.AudioChannels = combo.Channels;
                        tokens[i].Matched = true;
                        tokens[next].Matched = true;
                        return;
                    }
                }
            }
        }
    }
}
