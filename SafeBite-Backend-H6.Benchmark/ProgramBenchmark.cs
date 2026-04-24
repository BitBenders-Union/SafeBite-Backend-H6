using BenchmarkDotNet.Running;
using SafeBite_Backend_H6.Benchmark.Allergies;

// https://benchmarkdotnet.org/articles/guides/getting-started.html
// follow this guide to understand what's happening

public partial class ProgramBenchmark
{
    // https://benchmarkdotnet.org/articles/guides/how-to-run.html
    private static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(ProgramBenchmark).Assembly).Run(args);
    // rename Program.cs to something else to avoid conflict with api.program.cs

}
