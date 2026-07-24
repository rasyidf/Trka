# Terka (GuessIt.NET)

[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A .NET library that extracts media properties from video filenames. C# port of the Python [guessit](https://github.com/guessit-io/guessit) library.

> **Note:** The original C# port (v0.1) is preserved in the [`v0.1` branch](https://github.com/rasyidf/Trka/tree/v0.1). This is a complete rewrite targeting modern .NET.

## Author

**Rasyid F.** ([@rasyidf](https://github.com/rasyidf))

## Usage

```csharp
using Terka;

var result = GuessIt.Guess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv");
// result.Title       = "The Matrix"
// result.Year        = 1999
// result.ScreenSize  = "1080p"
// result.Source      = "Blu-ray"
// result.VideoCodec  = "H.264"
// result.ReleaseGroup = "GROUP"
// result.Container   = "mkv"
// result.Type        = MediaType.Movie
// result.Confidence  = 1.0

var episode = GuessIt.Guess("Shameless.US.S05E10.720p.HDTV.x264-KILLERS.mkv");
// episode.Title      = "Shameless"
// episode.Country    = "US"
// episode.Season     = [5]
// episode.Episode    = [10]
// episode.Type       = MediaType.Episode

// Anime-style with CRC32
var anime = GuessIt.Guess("[SubGroup] Attack on Titan - 25 [1080p][HEVC][A1B2C3D4].mkv");
// anime.Title           = "Attack on Titan"
// anime.AbsoluteEpisode = [25]
// anime.ReleaseGroup    = "SubGroup"
// anime.VideoCodec      = "H.265"
// anime.Crc32           = "A1B2C3D4"

// Episode with title and range
var ep = GuessIt.Guess("Breaking.Bad.S01E01-03.Pilot.720p.BluRay.x264-DEMAND.mkv");
// ep.Title        = "Breaking Bad"
// ep.Season       = [1]
// ep.Episode      = [1, 2, 3]
// ep.EpisodeTitle = "Pilot"
```

### High-Performance Span Variant

For hot paths, use `Terka.Span` which uses `ReadOnlySpan<char>`, `FrozenDictionary`, and `stackalloc` for minimal allocations:

```csharp
using Terka.Span;

var result = SpanGuessIt.Guess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv");
// Same output, 4.5x faster, 91% less memory
```

### Zero-Allocation Path

For absolute hot paths (batch processing, real-time pipelines), use the `ref struct` variant with **zero GC allocations**:

```csharp
using Terka.Span;

var result = SpanGuessIt.GuessZeroAlloc("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv");
// result.Title is ReadOnlySpan<char> (raw slice, no string allocation)
// result.VideoCodec is interned string (no alloc)
// Total heap allocations: 0 bytes

// Materialize to heap when needed:
SpanGuessResult heapResult = result.ToHeapResult();
```

### TryGuess & Options

```csharp
// TryGuess: returns false if input doesn't look like a media filename
if (GuessIt.TryGuess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv", out var result))
{
    Console.WriteLine(result.Title); // "The Matrix"
}

// Force episode detection
var result = GuessIt.Guess("filename.mkv", new GuessOptions
{
    Type = MediaType.Episode
});

// JSON serialization (AOT/trimmed-friendly)
using System.Text.Json;
using Terka.Span.Serialization;

var json = JsonSerializer.Serialize(result, TerkaJsonContext.Default.SpanGuessResult);
```

## Benchmarks

Tested on .NET 10.0, 30 mixed filenames (movies, episodes, anime):

| Method | Mean | StdDev | Allocated | vs Baseline |
|--------|------|--------|-----------|-------------|
| **Terka** (netstandard2.0) | 342 µs | 8.3 µs | 117 KB | 1.00x |
| **Terka.Span** (net10.0) | 77 µs | 1.9 µs | 11 KB | **4.5x faster, 91% less memory** |
| **Terka.Span ZeroAlloc** (ref struct) | 70 µs | 1.8 µs | **0 B** | **4.9x faster, zero alloc** |
| **Python guessit** (estimated) | ~60,000–150,000 µs | — | — | ~200–400x slower |

Run benchmarks yourself:

```bash
cd benchmarks/Terka.Benchmarks
dotnet run -c Release -- --filter *GuessItBenchmarks*
```

## Detected Properties

| Property | Examples |
|----------|----------|
| Title | Movie/show name |
| Year | 1999, 2020 |
| Season / Episode | S01E02, 1x03, S01E01-03 (ranges) |
| Absolute Episode | Anime-style: 25 |
| Episode Title | S01E01.Pilot → "Pilot" |
| Country | US, UK, AU, CA, DE, FR, JP, KR |
| Language | German, French, English, DL, Multi |
| Source | Blu-ray, HDTV, Web, DVD |
| Screen Size | 1080p, 720p, 2160p, 4320p |
| Video Codec | H.264, H.265, AV1, Xvid |
| Audio Codec | AAC, DTS, DTS-HD, Dolby Atmos, TrueHD |
| Audio Channels | 5.1, 7.1, 2.0 |
| Container | mkv, avi, mp4 |
| Release Group | Scene/fansub group |
| Streaming Service | Netflix, Disney+, Amazon Prime, HBO Max |
| Edition | Director's Cut, Extended, Remastered, IMAX |
| Color Depth | 10-bit, 8-bit, 12-bit |
| CRC32 | 8-char hex (anime releases) |
| Other | Remux, HDR10, Dolby Vision, Proper, Dual Audio |
| Confidence | 0.0–1.0 (matched tokens / total tokens) |

## Project Structure

```
src/
  Terka/              # Main library (netstandard2.0, zero dependencies)
  Terka.Span/         # High-perf Span<T> variant (net10.0, FrozenDictionary)
  Shared/             # Shared vocabulary (single source of truth)
tests/
  Terka.Tests/        # xUnit tests (93 tests, incl. property-based fuzzing)
benchmarks/
  Terka.Benchmarks/   # BenchmarkDotNet comparisons
```

## Installation

```
dotnet add package Terka
```

Or reference the project directly:
```xml
<ProjectReference Include="path/to/src/Terka/Terka.csproj" />
```

## Architecture

Both implementations share a single vocabulary (`src/Shared/Vocabulary.cs`) linked into both projects. Adding a new keyword requires editing one file.

| Layer | Terka (netstandard2.0) | Terka.Span (net10.0) |
|-------|------------------------|----------------------|
| Tokenizer | Regex split | Manual span scan, `stackalloc` |
| Matchers | `DictionaryMatcher` (two-token lookahead) | `FrozenDictionary` + `AlternateLookup<ReadOnlySpan<char>>` |
| Result | `class GuessResult` | `class SpanGuessResult` (lazy lists) / `ref struct ZeroAllocGuessResult` (InlineArray) |
| Allocations | ~117 KB/batch | 11 KB (heap) / 0 B (zero-alloc) |

## Contributing

Contributions welcome! Please open an issue or PR at [github.com/rasyidf/Trka](https://github.com/rasyidf/Trka).

## Acknowledgments

Based on the Python [guessit](https://github.com/guessit-io/guessit) library by guessit-io.

## License

MIT © [Rasyid F.](https://github.com/rasyidf)
