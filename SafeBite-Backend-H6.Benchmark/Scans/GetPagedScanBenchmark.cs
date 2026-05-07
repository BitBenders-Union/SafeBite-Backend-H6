
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    private string UserId = "";

    [GlobalSetup]
    public void GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config["ConnectionStrings:AppConnection"]
            ?? throw new InvalidOperationException("Connection string 'AppConnection' was not found.");

        UserId = config["User:UserId"]
            ?? throw new InvalidOperationException("Id string 'UserId' was not found");


        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        var repository = new ScanRepository(_dbContext);

        var ocrService = new FakeGetOcrService();
        var userAllergyService = new FakeGetUserAllergyAnalysisService();
        var scanAnalysisService = new FakeGetScanAnalysisService();
        var allergyMatcher = new FakeAllergyMatcher();

        _scanService = new ScanService(
            repository,
            ocrService,
            userAllergyService,
            scanAnalysisService,
            allergyMatcher,
            new LoggerFactory().CreateLogger<ScanService>()

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

    private sealed class FakeAllergyMatcher : IAllergyMatcher
    {
        public List<DetectedAllergyAnalysisResult> MatchAllergies(string ingredientsText, List<AllergyAnalysisItem> userAllergies)
        {
            throw new NotImplementedException("AllergyMatcher should not be called!");
        }

        public List<DetectedAllergyAnalysisResult> MatchLocalAllergies(string text, List<AllergyAnalysisItem> allergies)
        {
            throw new NotImplementedException("Local Allergy Matcher should not be called!");
        }

        public void MergeResults(ScanAnalysisResult ai, List<DetectedAllergyAnalysisResult> local)
        {
            throw new NotImplementedException("MergeResults should not be called!");
        }
    }

}


