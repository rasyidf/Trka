using System;
using System.Collections.Generic;
using Terka.Matchers;

namespace Terka
{
    /// <summary>
    /// Main entry point for guessing media file properties from a filename.
    /// Port of Python's guessit library.
    /// </summary>
    public static class GuessIt
    {
        // Matchers are stateless singletons (they only read from their internal dictionaries)
        private static readonly EpisodeMatcher _episodeMatcher = new EpisodeMatcher();
        private static readonly VideoCodecMatcher _videoCodecMatcher = new VideoCodecMatcher();
        private static readonly AudioCodecMatcher _audioCodecMatcher = new AudioCodecMatcher();
        private static readonly AudioChannelsMatcher _audioChannelsMatcher = new AudioChannelsMatcher();
        private static readonly ScreenSizeMatcher _screenSizeMatcher = new ScreenSizeMatcher();
        private static readonly SourceMatcher _sourceMatcher = new SourceMatcher();
        private static readonly EditionMatcher _editionMatcher = new EditionMatcher();
        private static readonly OtherMatcher _otherMatcher = new OtherMatcher();
        private static readonly YearMatcher _yearMatcher = new YearMatcher();
        private static readonly ReleaseGroupMatcher _releaseGroupMatcher = new ReleaseGroupMatcher();
        private static readonly StreamingServiceMatcher _streamingServiceMatcher = new StreamingServiceMatcher();
        private static readonly ColorDepthMatcher _colorDepthMatcher = new ColorDepthMatcher();
        private static readonly LanguageMatcher _languageMatcher = new LanguageMatcher();
        private static readonly ContainerMatcher _containerMatcher = new ContainerMatcher();
        private static readonly TitleExtractor _titleExtractor = new TitleExtractor();

        /// <summary>
        /// Guess media properties from a filename or release name.
        /// </summary>
        /// <param name="filename">The filename or release name to analyze.</param>
        /// <param name="options">Optional options to influence guessing.</param>
        /// <returns>A GuessResult containing all detected properties.</returns>
        public static GuessResult Guess(string filename, GuessOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));

            var result = new GuessResult();
            var tokenResult = Tokenizer.Tokenize(filename);
            var tokens = tokenResult.Tokens;

            // Container from extension
            _containerMatcher.Match(tokenResult.Extension, result);

            // Run matchers in priority order (most specific first)
            _episodeMatcher.Match(tokens, result);
            _videoCodecMatcher.Match(tokens, result);
            _audioCodecMatcher.Match(tokens, result);
            _audioChannelsMatcher.Match(tokens, result);
            _screenSizeMatcher.Match(tokens, result);
            _sourceMatcher.Match(tokens, result);
            _streamingServiceMatcher.Match(tokens, result);
            _editionMatcher.Match(tokens, result);
            _otherMatcher.Match(tokens, result);
            _colorDepthMatcher.Match(tokens, result);
            _languageMatcher.Match(tokens, result);
            _yearMatcher.Match(tokens, result);

            // Absolute episode (anime) - only if no S##E## found
            _episodeMatcher.MatchAbsolute(tokens, result);

            // CRC32: 8-char hex bracket group (e.g. [A1B2C3D4])
            foreach (var token in tokens)
            {
                if (!token.IsBracketGroup || token.Matched) continue;
                if (token.Value.Length == 8 && IsHexWithLetter(token.Value))
                {
                    result.Crc32 = token.Value;
                    token.Matched = true;
                    break;
                }
            }

            // Release group (last, since it uses remaining unmatched tokens)
            _releaseGroupMatcher.Match(tokens, result);

            // Title extraction (collect leading unmatched tokens)
            _titleExtractor.Extract(tokens, result);

            // Determine media type
            result.Type = DetermineType(result, options);

            return result;
        }

        /// <summary>
        /// Convenience overload that returns a dictionary matching the Python guessit output format.
        /// </summary>
        public static Dictionary<string, object> GuessDict(string filename, GuessOptions options = null)
        {
            return Guess(filename, options).ToDictionary();
        }

        /// <summary>
        /// Attempts to guess media properties. Returns false if the input doesn't appear to be a media filename.
        /// </summary>
        public static bool TryGuess(string filename, out GuessResult result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(filename) || filename.Length < 3) return false;

            bool hasStructure = false;
            foreach (var c in filename)
            {
                if (c == '.' || c == ' ' || c == '_' || c == '-' || c == '[' || c == '(')
                { hasStructure = true; break; }
            }
            if (!hasStructure) return false;

            var r = Guess(filename);
            if (r.Title == null || (r.VideoCodec == null && r.Source == null && r.ScreenSize == null && r.Season.Count == 0 && r.Episode.Count == 0 && r.Year == null))
                return false;

            result = r;
            return true;
        }

        /// <summary>
        /// Attempts to guess media properties with options. Returns false if the input doesn't appear to be a media filename.
        /// </summary>
        public static bool TryGuess(string filename, GuessOptions options, out GuessResult result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(filename) || filename.Length < 3) return false;

            bool hasStructure = false;
            foreach (var c in filename)
            {
                if (c == '.' || c == ' ' || c == '_' || c == '-' || c == '[' || c == '(')
                { hasStructure = true; break; }
            }
            if (!hasStructure) return false;

            var r = Guess(filename, options);
            if (r.Title == null || (r.VideoCodec == null && r.Source == null && r.ScreenSize == null && r.Season.Count == 0 && r.Episode.Count == 0 && r.Year == null))
                return false;

            result = r;
            return true;
        }

        private static MediaType DetermineType(GuessResult result, GuessOptions options)
        {
            if (options?.Type != null) return options.Type.Value;

            // If we found season or episode info, it's an episode
            if (result.Season.Count > 0 || result.Episode.Count > 0 || result.AbsoluteEpisode.Count > 0)
                return MediaType.Episode;

            return MediaType.Movie;
        }

        private static bool IsHexWithLetter(string s)
        {
            bool hasLetter = false;
            foreach (var c in s)
            {
                if ((c >= '0' && c <= '9')) continue;
                if ((c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')) { hasLetter = true; continue; }
                return false;
            }
            return hasLetter;
        }
    }

    /// <summary>
    /// Options to influence the guessing process.
    /// </summary>
    public class GuessOptions
    {
        /// <summary>Force the media type instead of auto-detecting.</summary>
        public MediaType? Type { get; set; }

        /// <summary>Expected title hints (helps disambiguation).</summary>
        public IList<string> ExpectedTitle { get; set; }
    }
}
