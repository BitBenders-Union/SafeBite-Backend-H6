using BenchmarkDotNet.Attributes;
using SafeBite_Backend_H6.API.Contracts.Responses.Allergies;


namespace SafeBite_Backend_H6.Benchmark.Allergies;

// https://benchmarkdotnet.org/articles/features/setup-and-cleanup.html

public class GetAllergyByNameBenchmark : BaseAllergyBenchmark
{
    [Params("Soy", "Shellfish", "Not_Found")]
    public string AllergyName { get; set; } = null!;


    [Benchmark]
    public async Task<AllergyResponse?> GetAllergyByName()
    {
        return await AllergyService.GetAllergyByNameAsync(AllergyName);
    }
}
