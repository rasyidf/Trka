using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects season/episode patterns like S01E02, 1x03, Season 1, Episode 5, etc.
    /// </summary>
    internal class EpisodeMatcher : IMatcher
    {
        // S01E02, S01E02E03, S01E02-E03, S01E01-03
        // Note: range numbers are validated in MatchSxE to avoid matching screen sizes
        private static readonly Regex SeasonEpisode = new Regex(
            @"S(\d{1,3})E(\d{1,4})(?:[-\s]*E(\d{1,4}))*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Separate pattern for S01E01-03 numeric range (only matches when isolated by non-digit)
        private static readonly Regex EpisodeRange = new Regex(
            @"S\d{1,3}E(\d{1,4})\s+(\d{1,3})(?:\s|$)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 1x03, 01x05
        private static readonly Regex CrossPattern = new Regex(
            @"(\d{1,2})x(\d{2,3})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Season 1 / Season 01
        private static readonly Regex SeasonWord = new Regex(
            @"Season[\s\._-]*(\d{1,3})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Episode 5 / Ep 05 / E05 (standalone)
        private static readonly Regex EpisodeWord = new Regex(
            @"(?:Episode|Ep)[\s\._-]*(\d{1,4})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Standalone episode number for anime: " - 25 " or " - 025 "
        private static readonly Regex AbsoluteEpisode = new Regex(
            @"^(\d{2,4})$",
            RegexOptions.Compiled);

        public void Match(Token[] tokens, GuessResult result)
        {
            // First pass: check combined S##E## on original name (since it may span token boundaries)
            // We re-join to check the original patterns
            var joined = JoinTokenValues(tokens);
            MatchSxE(joined, result, tokens);

            // Fallback: check each token individually for remaining patterns
            foreach (var token in tokens)
            {
                if (token.Matched) continue;

                var crossMatch = CrossPattern.Match(token.Value);
                if (crossMatch.Success && crossMatch.Length == token.Value.Length)
                {
                    AddSeason(result, int.Parse(crossMatch.Groups[1].Value));
                    AddEpisode(result, int.Parse(crossMatch.Groups[2].Value));
                    token.Matched = true;
                    continue;
                }

                var seasonWordMatch = SeasonWord.Match(token.Value);
                if (seasonWordMatch.Success && seasonWordMatch.Length == token.Value.Length)
                {
                    AddSeason(result, int.Parse(seasonWordMatch.Groups[1].Value));
                    token.Matched = true;
                    continue;
                }

                var epWordMatch = EpisodeWord.Match(token.Value);
                if (epWordMatch.Success && epWordMatch.Length == token.Value.Length)
                {
                    AddEpisode(result, int.Parse(epWordMatch.Groups[1].Value));
                    token.Matched = true;
                    continue;
                }
            }
        }

        private static void MatchSxE(string joined, GuessResult result, Token[] tokens)
        {
            var matches = SeasonEpisode.Matches(joined);
            foreach (Match m in matches)
            {
                int season = int.Parse(m.Groups[1].Value);
                int firstEp = int.Parse(m.Groups[2].Value);
                AddSeason(result, season);
                AddEpisode(result, firstEp);

                // Group 3: explicit E## (e.g., S01E01E02 or S01E01-E03)
                if (m.Groups[3].Success)
                {
                    int lastEp = int.Parse(m.Groups[3].Value);
                    if (lastEp > firstEp && lastEp - firstEp <= 50)
                    {
                        for (int ep = firstEp + 1; ep <= lastEp; ep++)
                            AddEpisode(result, ep);
                    }
                    else
                    {
                        AddEpisode(result, lastEp);
                    }
                }

                MarkOverlapping(tokens, m.Value);
            }

            // Check for numeric range: S01E01 03 (tokenizer split on dash)
            var rangeMatch = EpisodeRange.Match(joined);
            if (rangeMatch.Success && result.Episode.Count > 0)
            {
                int firstEp = int.Parse(rangeMatch.Groups[1].Value);
                int rangeEnd = int.Parse(rangeMatch.Groups[2].Value);
                // Only expand if range end > first ep and looks reasonable (not a resolution)
                if (rangeEnd > firstEp && rangeEnd - firstEp <= 50 && rangeEnd < 100)
                {
                    for (int ep = firstEp + 1; ep <= rangeEnd; ep++)
                        AddEpisode(result, ep);
                    // Mark the range token
                    foreach (var token in tokens)
                    {
                        if (!token.Matched && token.Value == rangeMatch.Groups[2].Value)
                        {
                            token.Matched = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to detect absolute episode numbers (anime-style, e.g., " - 25")
        /// Called separately after other matchers have consumed their tokens.
        /// </summary>
        public void MatchAbsolute(Token[] tokens, GuessResult result)
        {
            // Only look for absolute episodes if we have no season/episode yet
            if (result.Season.Count > 0 || result.Episode.Count > 0) return;

            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                var m = AbsoluteEpisode.Match(token.Value);
                if (m.Success)
                {
                    int val = int.Parse(m.Value);
                    // Heuristic: if the number is between 1 and 1999 and not a year,
                    // treat it as an absolute episode
                    if (val >= 1 && val <= 1999 && (val < 1920 || val > 2030))
                    {
                        result.AbsoluteEpisode.Add(val);
                        token.Matched = true;
                        // Only take the first unmatched number
                        break;
                    }
                }
            }
        }

        private static void AddSeason(GuessResult result, int season)
        {
            if (!result.Season.Contains(season))
                result.Season.Add(season);
        }

        private static void AddEpisode(GuessResult result, int episode)
        {
            if (!result.Episode.Contains(episode))
                result.Episode.Add(episode);
        }

        private static string JoinTokenValues(Token[] tokens)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var t in tokens)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(t.Value);
            }
            return sb.ToString();
        }

        private static void MarkOverlapping(Token[] tokens, string matchedText)
        {
            var parts = matchedText.Split([' ', '.', '_', '-'],
                System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (token.Matched) continue;
                foreach (var part in parts)
                {
                    if (string.Equals(token.Value, part, System.StringComparison.OrdinalIgnoreCase) ||
                        token.Value.IndexOf(part, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        token.Matched = true;
                        break;
                    }
                }
            }
        }
    }
}
