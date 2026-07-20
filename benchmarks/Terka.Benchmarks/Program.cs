using BenchmarkDotNet.Running;
using Terka.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(GuessItBenchmarks).Assembly).Run(args);
