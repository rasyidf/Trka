"""
Benchmark script for Python guessit library.
Compares against the C# Terka implementations using the same test filenames.

Usage:
    pip install guessit
    python benchmark_guessit.py
"""

import timeit
import statistics
import sys

try:
    from guessit import guessit
except ImportError:
    print("Error: guessit not installed. Run: pip install guessit")
    sys.exit(1)

# Same filenames as C# benchmarks (TestFilenames.cs)
TEST_FILENAMES = [
    # Movies
    "The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv",
    "Inception.2010.2160p.UHD.BluRay.DTS-HD.x265-TERMiNAL.mkv",
    "Blade.Runner.2049.2017.Remastered.720p.BluRay.DTS.x264-DEMAND.mkv",
    "Interstellar.2014.IMAX.2160p.UHD.BluRay.Remux.HDR10.HEVC.Atmos-GROUP.mkv",
    "The.Shawshank.Redemption.1994.Directors.Cut.1080p.BluRay.x264-AMIABLE.mkv",
    "Dune.Part.Two.2024.2160p.AMZN.WEB-DL.DDP5.1.x265-FLUX.mkv",
    "Oppenheimer.2023.IMAX.1080p.BluRay.10bit.x265-GROUP.mkv",
    "Parasite.2019.720p.BluRay.x264-SPARKS.mkv",
    "Everything.Everywhere.All.at.Once.2022.1080p.AMZN.WEB-DL.DDP5.1.H.264-CMRG.mkv",
    "The.Godfather.1972.Remastered.2160p.UHD.BluRay.x265-TERMiNAL.mkv",
    # TV Episodes
    "Shameless.US.S05E10.720p.HDTV.x264-KILLERS.mkv",
    "The.Mandalorian.S02E01.2160p.DSNP.WEB-DL.DDP5.1.x265-GROUP.mkv",
    "Breaking.Bad.S01E01.Pilot.720p.BluRay.x264-DEMAND.mkv",
    "Game.of.Thrones.S08E06.The.Iron.Throne.1080p.AMZN.WEB-DL.DDP5.1.H.264-GoT.mkv",
    "Stranger.Things.S04E09.Chapter.Nine.The.Piggyback.2160p.NF.WEB-DL.DDP5.1.Atmos.DV.x265-FLUX.mkv",
    "House.of.the.Dragon.S01E10.1080p.HMAX.WEB-DL.DDP5.1.Atmos.H.264-FLUX.mkv",
    "The.Last.of.Us.S01E01.When.Youre.Lost.in.the.Darkness.1080p.HMAX.WEB-DL.DDP5.1.x264-NTb.mkv",
    "Treme.1x03.Right.Place.Wrong.Time.HDTV.XviD-NoTV.avi",
    "Friends.S01E01.The.Pilot.DVDRip.x264-HANNIBAL.mkv",
    "Severance.S01E09.The.We.We.Are.2160p.ATVP.WEB-DL.DDP5.1.DV.H.265-NTb.mkv",
    # Anime
    "[SubsPlease] Attack on Titan - 25 [1080p][HEVC].mkv",
    "[Erai-raws] Demon Slayer - 01 [1080p][HEVC].mkv",
    "[HorribleSubs] My Hero Academia - 88 [720p].mkv",
    "[Judas] One Piece - 1000 [1080p][HEVC 10bit].mkv",
    "[SubsPlease] Jujutsu Kaisen - 24 [1080p].mkv",
    # Edge cases
    "Movie.2020.Remux.2160p.BluRay.x265.mkv",
    "Some.Documentary.2023.PROPER.720p.WEB.H265-GROUP.mkv",
    "Concert.2023.Complete.MBluRay.1080p.DTS-HD.x264-NOPE.mkv",
    "Film.2021.Extended.Cut.1080p.BluRay.DDP.7.1.x265-EDGE.mkv",
    "Show.S01E01E02.720p.HDTV.mkv",
]


def bench_all_filenames():
    """Parse all test filenames once."""
    for f in TEST_FILENAMES:
        guessit(f)


def bench_single(filename):
    """Parse a single filename."""
    return guessit(filename)


def run_benchmark():
    print("=" * 70)
    print("Python guessit Benchmark")
    print("=" * 70)
    print(f"guessit version: {__import__('guessit').__version__}")
    print(f"Python version: {sys.version}")
    print(f"Test filenames: {len(TEST_FILENAMES)}")
    print()

    # Warmup
    print("Warming up...")
    for _ in range(3):
        bench_all_filenames()

    # Batch benchmark: all filenames
    print("\n--- Batch: All 30 filenames ---")
    iterations = 50
    times = timeit.repeat(bench_all_filenames, number=iterations, repeat=5)
    per_iteration = [t / iterations for t in times]
    per_file = [t / iterations / len(TEST_FILENAMES) for t in times]

    print(f"  Total (30 files): {statistics.mean(per_iteration)*1000:.2f} ms "
          f"(±{statistics.stdev(per_iteration)*1000:.2f} ms)")
    print(f"  Per file:         {statistics.mean(per_file)*1_000_000:.1f} µs "
          f"(±{statistics.stdev(per_file)*1_000_000:.1f} µs)")

    # Single file benchmarks
    print("\n--- Single file benchmarks ---")
    test_cases = [
        ("Movie", "The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv"),
        ("Episode", "The.Mandalorian.S02E01.2160p.DSNP.WEB-DL.DDP5.1.x265-GROUP.mkv"),
        ("Anime", "[SubsPlease] Attack on Titan - 25 [1080p][HEVC].mkv"),
    ]

    for label, filename in test_cases:
        times = timeit.repeat(lambda f=filename: guessit(f), number=500, repeat=5)
        per_call = [t / 500 for t in times]
        print(f"  {label:10s}: {statistics.mean(per_call)*1_000_000:.1f} µs "
              f"(±{statistics.stdev(per_call)*1_000_000:.1f} µs)")

    # Correctness check
    print("\n--- Correctness sample ---")
    r = guessit("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv")
    print(f"  Title: {r.get('title')}")
    print(f"  Year: {r.get('year')}")
    print(f"  Source: {r.get('source')}")
    print(f"  Video Codec: {r.get('video_codec')}")
    print(f"  Release Group: {r.get('release_group')}")

    print("\n" + "=" * 70)
    print("Done. Compare these timings with the C# BenchmarkDotNet results.")
    print("Run C# benchmarks: dotnet run -c Release -- --filter *GuessItBenchmarks*")
    print("=" * 70)


if __name__ == "__main__":
    run_benchmark()
