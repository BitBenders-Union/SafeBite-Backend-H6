
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SafeBite_Backend_H6.API.Contracts.Requests.Analysis;
using SafeBite_Backend_H6.API.Contracts.Responses.Analysis;
using SafeBite_Backend_H6.API.Contracts.Responses.OCR;
using SafeBite_Backend_H6.API.Contracts.Responses.Scans;
using SafeBite_Backend_H6.API.Data.AppDb;
using SafeBite_Backend_H6.API.Interfaces.Services;
using SafeBite_Backend_H6.API.Interfaces.Services.OCR;
using SafeBite_Backend_H6.API.Repositories;
using SafeBite_Backend_H6.API.Services.Scans;
using SafeBite_Backend_H6.API.Shared;

namespace SafeBite_Backend_H6.Benchmark.Scans;

[RPlotExporter]
public class GetPagedScanBenchmark
{
    private AppDbContext _dbContext = null!;
    private IScanService _scanService = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<GetPagedScanBenchmark>(optional: false)
            .Build();

        var connectionString = config["ConnectionStrings:AppConnection"]
            ?? throw new InvalidOperationException("Connection string 'AppConnection' was not found.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        var repository = new ScanRepository(_dbContext);

        var ocrService = new FakeGetOcrService();
        var userAllergyService = new FakeGetUserAllergyAnalysisService();
        var scanAnalysisService = new FakeGetScanAnalysisService();

        _scanService = new ScanService(
            repository,
            ocrService,
            userAllergyService,
            scanAnalysisService
        );
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _dbContext.Dispose();
    }

    [Params(5, 10, 20)]
    public int PageSize { get; set; }

    [Params("", "s", "soy")]
    public string? SearchTerm { get; set; }

    private const string UserId = "f9e29bed-095e-4cb4-aec8-8d69acf528b0";


    [Benchmark]
    public async Task<PagedResult<ScanResponse>> GetScansPagedAsync()
    {
        // create small overhead but it shouldn't affect the result much.
        var parameters = new PaginationParameters
        {
            Page = 1,
            PageSize = PageSize
        };

        return await _scanService.GetPagedByUserIdAsync(UserId, parameters, SearchTerm);
    }

    private sealed class FakeGetOcrService : IOcrService
    {
        public Task<OcrResponseDto> ExtractTextFromImageAsync(Stream imageStream, string? languages = null)
        {
            throw new NotImplementedException("OcrService Should not be called!");
        }
    }

    private sealed class FakeGetUserAllergyAnalysisService : IUserAllergyAnalysisService
    {
        public Task<List<AllergyAnalysisItem>> GetAllAllergiesForUserAsync(string userId)
        {
            throw new NotImplementedException("AllergyAnalysisService Should not be called!");
        }
    }

    private sealed class FakeGetScanAnalysisService : IScanAnalysisService
    {
        public Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request)
        {
            throw new NotImplementedException("ScanAnalysisService Should not be called!");
        }
    }

}


