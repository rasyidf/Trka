using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;

namespace Terka.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[HideColumns(Column.Error)]
public class GuessItBenchmarks
{
    private string[] _filenames = null!;

    [GlobalSetup]
    public void Setup()
    {
        _filenames = TestFilenames.All;

        // Warmup: ensure static initializers are done
        GuessIt.Guess(_filenames[0]);
        Span.SpanGuessIt.Guess(_filenames[0]);
    }

    [Benchmark(Baseline = true, Description = "Terka (netstandard2.0)")]
    public int Terka_Original()
    {
        int count = 0;
        foreach (var f in _filenames)
        {
            var result = GuessIt.Guess(f);
            if (result.Title != null) count++;
        }
        return count;
    }

    [Benchmark(Description = "Terka.Span (net10.0, Span<T>)")]
    public int Terka_Span()
    {
        int count = 0;
        foreach (var f in _filenames)
        {
            var result = Span.SpanGuessIt.Guess(f);
            if (result.Title != null) count++;
        }
        return count;
    }

    [Benchmark(Description = "Terka.Span ZeroAlloc (ref struct)")]
    public int Terka_ZeroAlloc()
    {
        int count = 0;
        foreach (var f in _filenames)
        {
            var result = Span.SpanGuessIt.GuessZeroAlloc(f);
            if (!result.Title.IsEmpty) count++;
        }
        return count;
    }
}

[MemoryDiagnoser]
[HideColumns(Column.Error)]
public class SingleFileBenchmarks
{
    private const string MovieFile = "The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv";
    private const string EpisodeFile = "The.Mandalorian.S02E01.2160p.DSNP.WEB-DL.DDP5.1.x265-GROUP.mkv";
    private const string AnimeFile = "[SubsPlease] Attack on Titan - 25 [1080p][HEVC].mkv";

    [GlobalSetup]
    public void Setup()
    {
        GuessIt.Guess(MovieFile);
        Span.SpanGuessIt.Guess(MovieFile);
    }

    [Benchmark(Description = "Original - Movie")]
    public string? Original_Movie() => GuessIt.Guess(MovieFile).Title;

    [Benchmark(Description = "Span - Movie")]
    public string? Span_Movie() => Span.SpanGuessIt.Guess(MovieFile).Title;

    [Benchmark(Description = "Original - Episode")]
    public string? Original_Episode() => GuessIt.Guess(EpisodeFile).Title;

    [Benchmark(Description = "Span - Episode")]
    public string? Span_Episode() => Span.SpanGuessIt.Guess(EpisodeFile).Title;

    [Benchmark(Description = "Original - Anime")]
    public string? Original_Anime() => GuessIt.Guess(AnimeFile).Title;

    [Benchmark(Description = "Span - Anime")]
    public string? Span_Anime() => Span.SpanGuessIt.Guess(AnimeFile).Title;
}
