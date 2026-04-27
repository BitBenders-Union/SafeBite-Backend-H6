using BenchmarkDotNet.Attributes;
using SafeBite_Backend_H6.API.Contracts.Responses.Allergies;
using SafeBite_Backend_H6.API.Shared;


namespace SafeBite_Backend_H6.Benchmark.Allergies;

[RPlotExporter]
public class GetPagedAllergyBencmark : BaseAllergyBenchmark
{
    [Params(5, 10, 20)]
    public int PageSize { get; set; }

    [Params("", "s", "soy")]
    public string? SearchTerm { get; set; }

    [Benchmark]
    public async Task<PagedResult<AllergyResponse>> GetAllergiesPaged()
    {
        // create small overhead but it shouldn't affect the result much.
        var parameters = new PaginationParameters
        {
            Page = 1,
            PageSize = PageSize
        };

        return await AllergyService.GetAllergiesPagedAsync(parameters, SearchTerm);
    }
}
