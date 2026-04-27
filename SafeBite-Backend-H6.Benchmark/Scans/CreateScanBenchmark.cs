

using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SafeBite_Backend_H6.API.Contracts.Requests.Analysis;
using SafeBite_Backend_H6.API.Contracts.Requests.Scans;
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

public class CreateScanBenchmark
{
    private AppDbContext _dbContext = null!;
    private IScanService _scanService = null!;
    private IFormFile _imageFile = null!;

    private static readonly Guid PeanutAllergyId =
            Guid.Parse("019dab0c-d468-71e9-8f4e-69c7c8f2266b");

    private const string UserId = "f9e29bed-095e-4cb4-aec8-8d69acf528b0";

    [Params("Benchmark scan")]
    public string Name { get; set; } = null!;

    [Params("en")]
    public string Lang { get; set; } = null!;



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

        var ocrService = new FakeCreateOcrService();
        var userAllergyService = new FakeCreateUserAllergyAnalysisService();
        var scanAnalysisService = new FakeCreateScanAnalysisService();

        _scanService = new ScanService(
            repository,
            ocrService,
            userAllergyService,
            scanAnalysisService
        );
        _imageFile = CreateFakeImageFile();
    }


    [Benchmark]
    public async Task<ScanResponse> CreateScanAsync()
    {
        var request = new CreateScanRequest
        {
            Name = Name,
            Lang = Lang,
            Image = _imageFile
        };

        return await _scanService.CreateAsync(UserId, request);
    }

    private static IFormFile CreateFakeImageFile()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "Image", "benchmark-image.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    private sealed class FakeCreateOcrService : IOcrService
    {
        public Task<OcrResponseDto> ExtractTextFromImageAsync(Stream imageStream, string? languages = null)
        {
            return Task.FromResult(new OcrResponseDto
            {
                Language = "en",
                Status = "success",
                IngredientsText = "Ingredients: Water, Sugar, Peanut"
            });
        }
    }

    private sealed class FakeCreateUserAllergyAnalysisService : IUserAllergyAnalysisService
    {
        public Task<List<AllergyAnalysisItem>> GetAllAllergiesForUserAsync(string userId)
        {
            return Task.FromResult(new List<AllergyAnalysisItem>
            {
                new AllergyAnalysisItem
                {
                    AllergyId = PeanutAllergyId,
                    AllergyName = "Peanut"
                }
            });
        }
    }

    private sealed class FakeCreateScanAnalysisService : IScanAnalysisService
    {
        public Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request)
        {
            return Task.FromResult(new ScanAnalysisResult
            {
                IngredientsText = request.IngredientsText,
                DetectedAllergies = new List<DetectedAllergyAnalysisResult>
                {
                    new DetectedAllergyAnalysisResult
                    {
                        AllergyId = PeanutAllergyId,
                        AllergyName = "Peanut"
                    }
                }
            });
        }
    }


    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _dbContext.Dispose();
    }

}


