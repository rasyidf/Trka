using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Terka.Tests;

/// <summary>
/// Property-based fuzzing: generate filenames from known properties, parse, assert round-trip.
/// Uses deterministic random with fixed seed for reproducibility.
/// </summary>
public class FuzzingTests
{
    private static readonly string[] Titles = { "The Matrix", "Breaking Bad", "Inception", "Stranger Things", "Attack on Titan", "Dune", "Parasite", "Severance" };
    private static readonly int[] Years = { 1999, 2008, 2010, 2016, 2019, 2021, 2023, 2024 };
    private static readonly string[] ScreenSizes = { "480p", "720p", "1080p", "2160p" };
    private static readonly string[] Sources = { "BluRay", "HDTV", "WEBRip", "DVDRip" };
    private static readonly string[] SourceCanonical = { "Blu-ray", "HDTV", "Web", "DVD" };
    private static readonly string[] VideoCodecs = { "x264", "x265", "HEVC", "AV1" };
    private static readonly string[] VideoCodecCanonical = { "H.264", "H.265", "H.265", "AV1" };
    private static readonly string[] AudioCodecs = { "AAC", "DTS", "FLAC", "AC3" };
    private static readonly string[] AudioCodecCanonical = { "AAC", "DTS", "FLAC", "Dolby Digital" };
    private static readonly string[] Groups = { "GROUP", "SPARKS", "FLUX", "NTb", "DEMAND" };
    private static readonly string[] Containers = { "mkv", "avi", "mp4" };

    [Fact]
    public void Fuzz_Movies_SpanRoundTrip()
    {
        var rng = new Random(42);
        int passed = 0;

        for (int i = 0; i < 100; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var year = Years[rng.Next(Years.Length)];
            var screenIdx = rng.Next(ScreenSizes.Length);
            var srcIdx = rng.Next(Sources.Length);
            var codecIdx = rng.Next(VideoCodecs.Length);
            var group = Groups[rng.Next(Groups.Length)];
            var container = Containers[rng.Next(Containers.Length)];

            var filename = $"{title.Replace(" ", ".")}.{year}.{ScreenSizes[screenIdx]}.{Sources[srcIdx]}.{VideoCodecs[codecIdx]}-{group}.{container}";

            var result = Terka.Span.SpanGuessIt.Guess(filename);

            Assert.Equal(title, result.Title);
            Assert.Equal(year, result.Year);
            Assert.Equal(ScreenSizes[screenIdx].ToLowerInvariant(), result.ScreenSize);
            Assert.Equal(SourceCanonical[srcIdx], result.Source);
            Assert.Equal(VideoCodecCanonical[codecIdx], result.VideoCodec);
            Assert.Equal(group, result.ReleaseGroup);
            Assert.Equal(container, result.Container);
            Assert.Equal(Terka.Span.MediaType.Movie, result.Type);
            Assert.True(result.Confidence > 0.5f, $"Low confidence {result.Confidence} for: {filename}");
            passed++;
        }

        Assert.Equal(100, passed);
    }

    [Fact]
    public void Fuzz_Movies_BaseRoundTrip()
    {
        var rng = new Random(42);
        int passed = 0;

        for (int i = 0; i < 100; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var year = Years[rng.Next(Years.Length)];
            var screenIdx = rng.Next(ScreenSizes.Length);
            var srcIdx = rng.Next(Sources.Length);
            var codecIdx = rng.Next(VideoCodecs.Length);
            var group = Groups[rng.Next(Groups.Length)];
            var container = Containers[rng.Next(Containers.Length)];

            var filename = $"{title.Replace(" ", ".")}.{year}.{ScreenSizes[screenIdx]}.{Sources[srcIdx]}.{VideoCodecs[codecIdx]}-{group}.{container}";

            var result = Terka.GuessIt.Guess(filename);

            Assert.Equal(title, result.Title);
            Assert.Equal(year, result.Year);
            Assert.Equal(ScreenSizes[screenIdx].ToLowerInvariant(), result.ScreenSize);
            Assert.Equal(SourceCanonical[srcIdx], result.Source);
            Assert.Equal(VideoCodecCanonical[codecIdx], result.VideoCodec);
            Assert.Equal(group, result.ReleaseGroup);
            Assert.Equal(container, result.Container);
            Assert.Equal(Terka.MediaType.Movie, result.Type);
            Assert.True(result.Confidence > 0.5f, $"Low confidence {result.Confidence} for: {filename}");
            passed++;
        }

        Assert.Equal(100, passed);
    }

    [Fact]
    public void Fuzz_Episodes_SpanRoundTrip()
    {
        var rng = new Random(123);
        int passed = 0;

        for (int i = 0; i < 100; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var season = rng.Next(1, 10);
            var episode = rng.Next(1, 24);
            var screenIdx = rng.Next(ScreenSizes.Length);
            var srcIdx = rng.Next(Sources.Length);
            var codecIdx = rng.Next(VideoCodecs.Length);
            var group = Groups[rng.Next(Groups.Length)];
            var container = Containers[rng.Next(Containers.Length)];

            var filename = $"{title.Replace(" ", ".")}.S{season:D2}E{episode:D2}.{ScreenSizes[screenIdx]}.{Sources[srcIdx]}.{VideoCodecs[codecIdx]}-{group}.{container}";

            var result = Terka.Span.SpanGuessIt.Guess(filename);

            Assert.Equal(title, result.Title);
            Assert.Contains(season, result.Season);
            Assert.Contains(episode, result.Episode);
            Assert.Equal(ScreenSizes[screenIdx].ToLowerInvariant(), result.ScreenSize);
            Assert.Equal(SourceCanonical[srcIdx], result.Source);
            Assert.Equal(VideoCodecCanonical[codecIdx], result.VideoCodec);
            Assert.Equal(group, result.ReleaseGroup);
            Assert.Equal(container, result.Container);
            Assert.Equal(Terka.Span.MediaType.Episode, result.Type);
            Assert.True(result.Confidence > 0.5f);
            passed++;
        }

        Assert.Equal(100, passed);
    }

    [Fact]
    public void Fuzz_WithAudioCodec_RoundTrip()
    {
        var rng = new Random(999);
        int passed = 0;

        for (int i = 0; i < 50; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var year = Years[rng.Next(Years.Length)];
            var screenIdx = rng.Next(ScreenSizes.Length);
            var srcIdx = rng.Next(Sources.Length);
            var codecIdx = rng.Next(VideoCodecs.Length);
            var audioIdx = rng.Next(AudioCodecs.Length);
            var group = Groups[rng.Next(Groups.Length)];
            var container = Containers[rng.Next(Containers.Length)];

            var filename = $"{title.Replace(" ", ".")}.{year}.{ScreenSizes[screenIdx]}.{Sources[srcIdx]}.{AudioCodecs[audioIdx]}.{VideoCodecs[codecIdx]}-{group}.{container}";

            var result = Terka.Span.SpanGuessIt.Guess(filename);

            Assert.Equal(title, result.Title);
            Assert.Equal(year, result.Year);
            Assert.Equal(AudioCodecCanonical[audioIdx], result.AudioCodec);
            Assert.Equal(VideoCodecCanonical[codecIdx], result.VideoCodec);
            Assert.True(result.Confidence > 0.5f);
            passed++;
        }

        Assert.Equal(50, passed);
    }

    [Fact]
    public void Fuzz_Parity_BothImplementationsAgree()
    {
        var rng = new Random(777);
        int agreed = 0;

        for (int i = 0; i < 50; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var year = Years[rng.Next(Years.Length)];
            var screenIdx = rng.Next(ScreenSizes.Length);
            var srcIdx = rng.Next(Sources.Length);
            var codecIdx = rng.Next(VideoCodecs.Length);
            var group = Groups[rng.Next(Groups.Length)];
            var container = Containers[rng.Next(Containers.Length)];

            var filename = $"{title.Replace(" ", ".")}.{year}.{ScreenSizes[screenIdx]}.{Sources[srcIdx]}.{VideoCodecs[codecIdx]}-{group}.{container}";

            var b = Terka.GuessIt.Guess(filename);
            var s = Terka.Span.SpanGuessIt.Guess(filename);

            Assert.Equal(b.Title, s.Title);
            Assert.Equal(b.Year, s.Year);
            Assert.Equal(b.ScreenSize, s.ScreenSize);
            Assert.Equal(b.Source, s.Source);
            Assert.Equal(b.VideoCodec, s.VideoCodec);
            Assert.Equal(b.ReleaseGroup, s.ReleaseGroup);
            Assert.Equal(b.Container, s.Container);
            agreed++;
        }

        Assert.Equal(50, agreed);
    }

    [Fact]
    public void Fuzz_CountryDetection_Episodes()
    {
        string[] countries = { "US", "UK", "AU", "CA" };
        var rng = new Random(555);

        for (int i = 0; i < 20; i++)
        {
            var title = Titles[rng.Next(Titles.Length)];
            var country = countries[rng.Next(countries.Length)];
            var season = rng.Next(1, 8);
            var episode = rng.Next(1, 20);

            var filename = $"{title.Replace(" ", ".")}.{country}.S{season:D2}E{episode:D2}.720p.HDTV.x264-GRP.mkv";

            var s = Terka.Span.SpanGuessIt.Guess(filename);
            var b = Terka.GuessIt.Guess(filename);

            Assert.Equal(title, s.Title);
            Assert.Equal(country, s.Country);
            Assert.Contains(season, s.Season);

            Assert.Equal(title, b.Title);
            Assert.Equal(country, b.Country);
            Assert.Contains(season, b.Season);
        }
    }
}
