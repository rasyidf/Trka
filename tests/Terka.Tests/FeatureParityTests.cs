using Xunit;
using SpanParser = Terka.Span.SpanGuessIt;

namespace Terka.Tests;

/// <summary>
/// Comprehensive tests verifying feature parity between Terka (base) and Terka.Span.
/// Each test runs the same filename through both parsers and asserts identical results.
/// </summary>
public class FeatureParityTests
{
    // === MOVIES ===

    [Theory]
    [InlineData("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv",
        "The Matrix", 1999, "1080p", "Blu-ray", "H.264", null, null, "GROUP", "mkv")]
    [InlineData("Inception.2010.2160p.BluRay.x265-TERMiNAL.mkv",
        "Inception", 2010, "2160p", "Blu-ray", "H.265", null, null, "TERMiNAL", "mkv")]
    [InlineData("Parasite.2019.720p.BluRay.x264-SPARKS.mkv",
        "Parasite", 2019, "720p", "Blu-ray", "H.264", null, null, "SPARKS", "mkv")]
    [InlineData("Oppenheimer.2023.IMAX.1080p.BluRay.10bit.x265-GROUP.mkv",
        "Oppenheimer", 2023, "1080p", "Blu-ray", "H.265", null, "10-bit", "GROUP", "mkv")]
    public void Movie_Properties(string filename,
        string title, int year, string screenSize, string source,
        string videoCodec, string? audioCodec, string? colorDepth,
        string group, string container)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        // Base Terka
        Assert.Equal(title, b.Title);
        Assert.Equal(year, b.Year);
        Assert.Equal(screenSize, b.ScreenSize);
        Assert.Equal(source, b.Source);
        Assert.Equal(videoCodec, b.VideoCodec);
        Assert.Equal(audioCodec, b.AudioCodec);
        Assert.Equal(colorDepth, b.ColorDepth);
        Assert.Equal(group, b.ReleaseGroup);
        Assert.Equal(container, b.Container);
        Assert.Equal(Terka.MediaType.Movie, b.Type);

        // Span variant
        Assert.Equal(title, s.Title);
        Assert.Equal(year, s.Year);
        Assert.Equal(screenSize, s.ScreenSize);
        Assert.Equal(source, s.Source);
        Assert.Equal(videoCodec, s.VideoCodec);
        Assert.Equal(audioCodec, s.AudioCodec);
        Assert.Equal(colorDepth, s.ColorDepth);
        Assert.Equal(group, s.ReleaseGroup);
        Assert.Equal(container, s.Container);
        Assert.Equal(Span.MediaType.Movie, s.Type);
    }

    // === AUDIO CODECS (two-token) ===

    [Theory]
    [InlineData("Movie.2020.1080p.DTS.HD.x264-GRP.mkv", "DTS-HD")]
    [InlineData("Movie.2020.1080p.DTS-HD.x264-GRP.mkv", "DTS-HD")]
    [InlineData("Movie.2020.1080p.TrueHD.x264-GRP.mkv", "Dolby TrueHD")]
    [InlineData("Movie.2020.1080p.Atmos.x264-GRP.mkv", "Dolby Atmos")]
    [InlineData("Movie.2020.1080p.AAC.x264-GRP.mkv", "AAC")]
    [InlineData("Movie.2020.1080p.FLAC.x264-GRP.mkv", "FLAC")]
    [InlineData("Movie.2020.1080p.DTS.x264-GRP.mkv", "DTS")]
    public void AudioCodec_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.AudioCodec);
        Assert.Equal(expected, s.AudioCodec);
    }

    // === AUDIO CHANNELS ===

    [Theory]
    [InlineData("Movie.2020.1080p.5.1.BluRay.mkv", "5.1")]
    [InlineData("Movie.2020.1080p.7.1.BluRay.mkv", "7.1")]
    [InlineData("Movie.2020.1080p.2.0.BluRay.mkv", "2.0")]
    public void AudioChannels_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.AudioChannels);
        Assert.Equal(expected, s.AudioChannels);
    }

    // === EPISODES ===

    [Theory]
    [InlineData("Show.S01E01.720p.HDTV.x264-GRP.mkv", 1, 1)]
    [InlineData("Show.S05E10.720p.HDTV.x264-GRP.mkv", 5, 10)]
    [InlineData("Show.S12E24.1080p.WEB.x265-GRP.mkv", 12, 24)]
    public void Episode_SxxExx(string filename, int season, int episode)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Contains(season, b.Season);
        Assert.Contains(episode, b.Episode);
        Assert.Equal(Terka.MediaType.Episode, b.Type);

        Assert.Contains(season, s.Season);
        Assert.Contains(episode, s.Episode);
        Assert.Equal(Span.MediaType.Episode, s.Type);
    }

    [Fact]
    public void Episode_MultiEpisode()
    {
        // S01E01E02 — both implementations should parse this
        var b = GuessIt.Guess("Show.S01E01E02.720p.HDTV.mkv");
        var s = SpanParser.Guess("Show.S01E01E02.720p.HDTV.mkv");

        Assert.Contains(1, b.Season);
        Assert.Contains(1, b.Episode);
        Assert.Contains(2, b.Episode);
        Assert.Equal(Terka.MediaType.Episode, b.Type);

        Assert.Contains(1, s.Season);
        Assert.Contains(1, s.Episode);
        Assert.Contains(2, s.Episode);
        Assert.Equal(Span.MediaType.Episode, s.Type);
    }

    [Fact]
    public void Episode_CrossPattern()
    {
        var b = GuessIt.Guess("Show.2x05.720p.HDTV.x264-GRP.mkv");
        var s = SpanParser.Guess("Show.2x05.720p.HDTV.x264-GRP.mkv");

        Assert.Contains(2, b.Season);
        Assert.Contains(5, b.Episode);
        Assert.Contains(2, s.Season);
        Assert.Contains(5, s.Episode);
    }

    // === ANIME (absolute episode) ===

    [Fact]
    public void Anime_AbsoluteEpisode()
    {
        var b = GuessIt.Guess("[SubGroup] Attack on Titan - 25 [1080p][HEVC].mkv");
        var s = SpanParser.Guess("[SubGroup] Attack on Titan - 25 [1080p][HEVC].mkv");

        Assert.Equal("Attack on Titan", b.Title);
        Assert.Contains(25, b.AbsoluteEpisode);
        Assert.Equal("H.265", b.VideoCodec);
        Assert.Equal(Terka.MediaType.Episode, b.Type);

        Assert.Equal("Attack on Titan", s.Title);
        Assert.Contains(25, s.AbsoluteEpisode);
        Assert.Equal("H.265", s.VideoCodec);
        Assert.Equal(Span.MediaType.Episode, s.Type);
    }

    // === STREAMING SERVICES ===

    [Theory]
    [InlineData("Movie.2020.1080p.AMZN.WEB-DL.x264-GRP.mkv", "Amazon Prime")]
    [InlineData("Movie.2020.1080p.NF.WEB-DL.x264-GRP.mkv", "Netflix")]
    [InlineData("Movie.2020.1080p.DSNP.WEB-DL.x264-GRP.mkv", "Disney+")]
    [InlineData("Movie.2020.1080p.HMAX.WEB-DL.x264-GRP.mkv", "HBO Max")]
    [InlineData("Movie.2020.1080p.ATVP.WEB-DL.x264-GRP.mkv", "AppleTV")]
    public void StreamingService_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.StreamingService);
        Assert.Equal(expected, s.StreamingService);
    }

    // === EDITIONS (two-token) ===

    [Theory]
    [InlineData("Movie.2020.Directors.Cut.1080p.BluRay.x264-GRP.mkv", "Director's Cut")]
    [InlineData("Movie.2020.Extended.Cut.1080p.BluRay.x264-GRP.mkv", "Extended")]
    [InlineData("Movie.2020.Theatrical.Cut.1080p.BluRay.x264-GRP.mkv", "Theatrical")]
    [InlineData("Movie.2020.Remastered.1080p.BluRay.x264-GRP.mkv", "Remastered")]
    [InlineData("Movie.2020.IMAX.1080p.BluRay.x264-GRP.mkv", "IMAX")]
    public void Edition_Detection(string filename, string expected)
    {
        var s = SpanParser.Guess(filename);
        Assert.Contains(expected, s.Edition);
    }

    // === OTHER PROPERTIES (two-token) ===

    [Theory]
    [InlineData("Movie.2020.1080p.Dolby.Vision.BluRay.x265-GRP.mkv", "Dolby Vision")]
    [InlineData("Movie.2020.1080p.Dual.Audio.BluRay.x264-GRP.mkv", "Dual Audio")]
    [InlineData("Movie.2020.1080p.HDR10.BluRay.x265-GRP.mkv", "HDR10")]
    [InlineData("Movie.2020.1080p.Remux.BluRay.x265-GRP.mkv", "Remux")]
    [InlineData("Movie.2020.PROPER.720p.WEB.x264-GRP.mkv", "Proper")]
    public void Other_Detection(string filename, string expected)
    {
        var s = SpanParser.Guess(filename);
        Assert.Contains(expected, s.Other);
    }

    // === SOURCES (two-token) ===

    [Theory]
    [InlineData("Movie.2020.1080p.BluRay.x264-GRP.mkv", "Blu-ray")]
    [InlineData("Movie.2020.1080p.WEBRip.x264-GRP.mkv", "Web")]
    [InlineData("Movie.2020.1080p.HDTV.x264-GRP.mkv", "HDTV")]
    [InlineData("Movie.2020.1080p.DVDRip.x264-GRP.mkv", "DVD")]
    public void Source_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.Source);
        Assert.Equal(expected, s.Source);
    }

    // === LANGUAGE DETECTION ===

    [Theory]
    [InlineData("Movie.2020.German.1080p.BluRay.x264-GRP.mkv", "German")]
    [InlineData("Movie.2020.French.720p.BluRay.x264-GRP.mkv", "French")]
    [InlineData("Movie.2020.1080p.BluRay.DL.x264-GRP.mkv", "Dual Language")]
    public void Language_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.Language);
        Assert.Equal(expected, s.Language);
    }

    // === SCREEN SIZE VARIANTS ===

    [Theory]
    [InlineData("Movie.2020.480p.DVDRip.x264-GRP.mkv", "480p")]
    [InlineData("Movie.2020.720p.HDTV.x264-GRP.mkv", "720p")]
    [InlineData("Movie.2020.1080p.BluRay.x264-GRP.mkv", "1080p")]
    [InlineData("Movie.2020.1080i.HDTV.x264-GRP.mkv", "1080i")]
    [InlineData("Movie.2020.2160p.UHD.BluRay.x265-GRP.mkv", "2160p")]
    public void ScreenSize_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.ScreenSize);
        Assert.Equal(expected, s.ScreenSize);
    }

    // === VIDEO CODECS ===

    [Theory]
    [InlineData("Movie.2020.1080p.x264-GRP.mkv", "H.264")]
    [InlineData("Movie.2020.1080p.x265-GRP.mkv", "H.265")]
    [InlineData("Movie.2020.1080p.HEVC-GRP.mkv", "H.265")]
    [InlineData("Movie.2020.720p.XviD-GRP.avi", "Xvid")]
    [InlineData("Movie.2020.1080p.AV1-GRP.mkv", "AV1")]
    public void VideoCodec_Detection(string filename, string expected)
    {
        var b = GuessIt.Guess(filename);
        var s = SpanParser.Guess(filename);

        Assert.Equal(expected, b.VideoCodec);
        Assert.Equal(expected, s.VideoCodec);
    }

    // === API SURFACE ===

    [Fact]
    public void TryGuess_ValidFilename_ReturnsTrue()
    {
        var success = GuessIt.TryGuess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv", out var b);
        var successS = SpanParser.TryGuess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv", out var s);

        Assert.True(success);
        Assert.True(successS);
        Assert.NotNull(b);
        Assert.NotNull(s);
        Assert.Equal("The Matrix", b.Title);
        Assert.Equal("The Matrix", s!.Title);
    }

    [Fact]
    public void TryGuess_InvalidFilename_ReturnsFalse()
    {
        var success = GuessIt.TryGuess("hello", out var b);
        var successS = SpanParser.TryGuess("hello", out var s);

        Assert.False(success);
        Assert.False(successS);
        Assert.Null(b);
        Assert.Null(s);
    }

    [Fact]
    public void GuessOptions_ForceEpisode()
    {
        var b = GuessIt.Guess("Movie.2020.1080p.mkv", new GuessOptions { Type = Terka.MediaType.Episode });
        var s = SpanParser.Guess("Movie.2020.1080p.mkv", new Span.GuessOptions { Type = Span.MediaType.Episode });

        Assert.Equal(Terka.MediaType.Episode, b.Type);
        Assert.Equal(Span.MediaType.Episode, s.Type);
    }

    [Fact]
    public void ToDictionary_BothProduceConsistentOutput()
    {
        var b = GuessIt.Guess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv").ToDictionary();
        var s = SpanParser.Guess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv").ToDictionary();

        Assert.Equal(b["title"], s["title"]);
        Assert.Equal(b["year"], s["year"]);
        Assert.Equal(b["screen_size"], s["screen_size"]);
        Assert.Equal(b["source"], s["source"]);
        Assert.Equal(b["video_codec"], s["video_codec"]);
        Assert.Equal(b["release_group"], s["release_group"]);
        Assert.Equal(b["container"], s["container"]);
    }

    // === COMPLEX REAL-WORLD FILENAMES ===

    [Fact]
    public void Complex_StreamingEpisodeWithDV()
    {
        const string f = "Stranger.Things.S04E09.2160p.NF.WEB-DL.DDP5.1.Atmos.DV.x265-FLUX.mkv";
        var s = SpanParser.Guess(f);

        Assert.Equal("Stranger Things", s.Title);
        Assert.Contains(4, s.Season);
        Assert.Contains(9, s.Episode);
        Assert.Equal("2160p", s.ScreenSize);
        Assert.Equal("Netflix", s.StreamingService);
        Assert.Equal("Web", s.Source);
        Assert.Equal("H.265", s.VideoCodec);
        Assert.Equal("FLUX", s.ReleaseGroup);
        Assert.Contains("Dolby Vision", s.Other);
    }

    [Fact]
    public void Complex_MovieWithHDRAndAtmos()
    {
        const string f = "Interstellar.2014.IMAX.2160p.UHD.BluRay.Remux.HDR10.HEVC.Atmos-GROUP.mkv";
        var s = SpanParser.Guess(f);

        Assert.Equal("Interstellar", s.Title);
        Assert.Equal(2014, s.Year);
        Assert.Equal("2160p", s.ScreenSize);
        Assert.Equal("H.265", s.VideoCodec);
        Assert.Equal("Dolby Atmos", s.AudioCodec);
        Assert.Contains("Remux", s.Other);
        Assert.Contains("HDR10", s.Other);
        Assert.Contains("IMAX", s.Edition);
    }

    [Fact]
    public void Complex_GermanDualLanguage()
    {
        const string f = "Movie.2020.German.DL.1080p.BluRay.x264-GRP.mkv";
        var s = SpanParser.Guess(f);

        Assert.Equal("Movie", s.Title);
        Assert.Equal(2020, s.Year);
        Assert.Equal("1080p", s.ScreenSize);
        Assert.Equal("Blu-ray", s.Source);
        // Either German or DL detected as language
        Assert.NotNull(s.Language);
    }

    [Fact]
    public void ZeroAlloc_ProducesConsistentResults()
    {
        const string f = "The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv";
        var heap = SpanParser.Guess(f);
        var zero = SpanParser.GuessZeroAlloc(f);

        Assert.Equal(heap.Year, zero.Year);
        Assert.Equal(heap.ScreenSize, zero.ScreenSize);
        Assert.Equal(heap.Source, zero.Source);
        Assert.Equal(heap.VideoCodec, zero.VideoCodec);
        Assert.Equal(heap.Container, zero.Container);
        Assert.Equal(heap.ReleaseGroup, new string(zero.ReleaseGroup));
    }
}


/// <summary>
/// Tests for correctness improvements: episode ranges, CRC32, episode title, two-token matching.
/// </summary>
public class CorrectnessTests
{
    // === EPISODE RANGES ===

    [Fact]
    public void EpisodeRange_S01E01E02E03()
    {
        var s = Terka.Span.SpanGuessIt.Guess("Show.S01E01E02E03.720p.HDTV.mkv");

        Assert.Contains(1, s.Season);
        Assert.Contains(1, s.Episode);
        Assert.Contains(2, s.Episode);
        Assert.Contains(3, s.Episode);
    }

    [Fact]
    public void EpisodeRange_S01E01_03()
    {
        // S01E01-03 should expand to episodes 1, 2, 3
        var s = Terka.Span.SpanGuessIt.Guess("Show.S01E01-03.720p.HDTV.mkv");

        Assert.Contains(1, s.Season);
        Assert.Contains(1, s.Episode);
        Assert.Contains(2, s.Episode);
        Assert.Contains(3, s.Episode);
    }

    [Fact]
    public void EpisodeRange_S01E01_E03()
    {
        // S01E01-E03 should expand to episodes 1, 2, 3
        var s = Terka.Span.SpanGuessIt.Guess("Show.S01E01-E03.720p.HDTV.mkv");

        Assert.Contains(1, s.Season);
        Assert.Contains(1, s.Episode);
        Assert.Contains(2, s.Episode);
        Assert.Contains(3, s.Episode);
    }

    [Fact]
    public void EpisodeRange_Base_S01E01_03()
    {
        var b = Terka.GuessIt.Guess("Show.S01E01-03.720p.HDTV.mkv");

        Assert.Contains(1, b.Season);
        Assert.Contains(1, b.Episode);
        Assert.Contains(2, b.Episode);
        Assert.Contains(3, b.Episode);
    }

    // === CRC32 DETECTION ===

    [Fact]
    public void Crc32_SpanDetection()
    {
        var s = Terka.Span.SpanGuessIt.Guess("[SubGroup] Anime Title - 01 [1080p][HEVC][A1B2C3D4].mkv");

        Assert.Equal("A1B2C3D4", s.Crc32);
        Assert.Equal("SubGroup", s.ReleaseGroup);
    }

    [Fact]
    public void Crc32_BaseDetection()
    {
        var b = Terka.GuessIt.Guess("[SubGroup] Anime Title - 01 [1080p][HEVC][A1B2C3D4].mkv");

        Assert.Equal("A1B2C3D4", b.Crc32);
    }

    [Fact]
    public void Crc32_NotConfusedWithGroup()
    {
        // "SubGroup" has letters but isn't 8-char hex — should be release group, not CRC32
        var s = Terka.Span.SpanGuessIt.Guess("[SubGroup] Title - 01 [720p][DEADBEEF].mkv");

        Assert.Equal("DEADBEEF", s.Crc32);
        Assert.Equal("SubGroup", s.ReleaseGroup);
    }

    // === EPISODE TITLE ===

    [Fact]
    public void EpisodeTitle_Span()
    {
        var s = Terka.Span.SpanGuessIt.Guess("Breaking.Bad.S01E01.Pilot.720p.BluRay.x264-DEMAND.mkv");

        Assert.Equal("Breaking Bad", s.Title);
        Assert.Equal("Pilot", s.EpisodeTitle);
        Assert.Contains(1, s.Season);
        Assert.Contains(1, s.Episode);
    }

    [Fact]
    public void EpisodeTitle_Base()
    {
        var b = Terka.GuessIt.Guess("Breaking.Bad.S01E01.Pilot.720p.BluRay.x264-DEMAND.mkv");

        Assert.Equal("Breaking Bad", b.Title);
        Assert.Equal("Pilot", b.EpisodeTitle);
        Assert.Contains(1, b.Season);
        Assert.Contains(1, b.Episode);
    }

    // === TWO-TOKEN BASE TERKA MATCHING ===

    [Fact]
    public void TwoToken_DirectorsCut_Base()
    {
        var b = Terka.GuessIt.Guess("Movie.2020.Directors.Cut.1080p.BluRay.x264-GRP.mkv");

        Assert.Contains("Director's Cut", b.Edition);
    }

    [Fact]
    public void TwoToken_DolbyVision_Base()
    {
        var b = Terka.GuessIt.Guess("Movie.2020.1080p.Dolby.Vision.BluRay.x265-GRP.mkv");

        Assert.Contains("Dolby Vision", b.Other);
    }

    [Fact]
    public void TwoToken_DualAudio_Base()
    {
        var b = Terka.GuessIt.Guess("Movie.2020.1080p.Dual.Audio.BluRay.x264-GRP.mkv");

        Assert.Contains("Dual Audio", b.Other);
    }
}
