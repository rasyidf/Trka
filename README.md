# Terka (GuessIt.NET)

[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A .NET Standard 2.0 library that extracts media properties from video filenames. C# port of the Python [guessit](https://github.com/guessit-io/guessit) library.

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

var episode = GuessIt.Guess("Shameless.US.S05E10.720p.HDTV.x264-KILLERS.mkv");
// episode.Title      = "Shameless US"
// episode.Season     = [5]
// episode.Episode    = [10]
// episode.Type       = MediaType.Episode

// Anime-style
var anime = GuessIt.Guess("[SubGroup] Attack on Titan - 25 [1080p][HEVC].mkv");
// anime.Title           = "Attack on Titan"
// anime.AbsoluteEpisode = [25]
// anime.ReleaseGroup    = "SubGroup"
// anime.VideoCodec      = "H.265"
```

## Detected Properties

| Property | Examples |
|----------|----------|
| Title | Movie/show name |
| Year | 1999, 2020 |
| Season / Episode | S01E02, 1x03 |
| Absolute Episode | Anime-style: 25 |
| Source | Blu-ray, HDTV, Web, DVD |
| Screen Size | 1080p, 720p, 2160p |
| Video Codec | H.264, H.265, Xvid |
| Audio Codec | AAC, DTS, DTS-HD, Dolby Atmos |
| Audio Channels | 5.1, 7.1, 2.0 |
| Container | mkv, avi, mp4 |
| Release Group | Scene/fansub group |
| Streaming Service | Netflix, Disney+, Amazon Prime |
| Edition | Director's Cut, Extended, Remastered |
| Color Depth | 10-bit, 8-bit |
| Other | Remux, HDR10, Dolby Vision, Proper |

## Installation

```
dotnet add package Terka
```

Or reference the project directly:
```xml
<ProjectReference Include="path/to/src/Terka/Terka.csproj" />
```

## Dictionary Output

```csharp
var dict = GuessIt.GuessDict("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv");
// Returns Dictionary<string, object> matching guessit Python output format
```

## Options

```csharp
var result = GuessIt.Guess("filename.mkv", new GuessOptions
{
    Type = MediaType.Episode // Force episode detection
});
```

## Contributing

Contributions welcome! Please open an issue or PR at [github.com/rasyidf/Trka](https://github.com/rasyidf/Trka).

## Acknowledgments

Based on the Python [guessit](https://github.com/guessit-io/guessit) library by guessit-io.

## License

MIT © [Rasyid F.](https://github.com/rasyidf)
