

using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    private static Guid PeanutAllergyId;
    private string UserId;

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

        UserId = config["User:UserId"]
            ?? throw new InvalidOperationException("Id string 'UserId' was not found");

        var peanutId = config["Allergy:PeanutAllergyId"]
            ?? throw new InvalidOperationException("PeanutAllergyId missing");

        PeanutAllergyId = Guid.Parse(peanutId);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        var repository = new ScanRepository(_dbContext);

        var ocrService = new FakeCreateOcrService();
        var userAllergyService = new FakeCreateUserAllergyAnalysisService();
        var scanAnalysisService = new FakeCreateScanAnalysisService();
        var allergyMatcher = new FakeAllergyMatcher();

        _scanService = new ScanService(
            repository,
            ocrService,
            userAllergyService,
            scanAnalysisService,
            allergyMatcher,
            new LoggerFactory().CreateLogger<ScanService>()

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

    private sealed class FakeAllergyMatcher : IAllergyMatcher
    {
        public List<DetectedAllergyAnalysisResult> MatchAllergies(string ingredientsText, List<AllergyAnalysisItem> userAllergies)
        {
            return new List<DetectedAllergyAnalysisResult>
            {
                new DetectedAllergyAnalysisResult
                {
                    AllergyId = PeanutAllergyId,
                    AllergyName = "Peanut",
                    MatchedIngredients = new List<string> { "Peanut" }
                }

            };
        }

        public List<DetectedAllergyAnalysisResult> MatchLocalAllergies(string text, List<AllergyAnalysisItem> allergies)
        {
            return new List<DetectedAllergyAnalysisResult>
            {
                new DetectedAllergyAnalysisResult
                {
                    AllergyId = PeanutAllergyId,
                    AllergyName = text,
                    MatchedIngredients = new List<string> { "Peanut" }
                }
            };
        }

        public void MergeResults(ScanAnalysisResult ai, List<DetectedAllergyAnalysisResult> local)
        {
            // we use the analysis result after this method is called therefore we need to ensure this variable is correct

            local = local ?? new List<DetectedAllergyAnalysisResult>
            {
                    new DetectedAllergyAnalysisResult
                    {
                        AllergyId = PeanutAllergyId,
                        AllergyName = "Peanut",
                        MatchedIngredients =new List<string> {
                            "Peanut"
                        },
                    }
            };

            ai = new ScanAnalysisResult
            {
                IngredientsText = ai.IngredientsText,
                DetectedAllergies = local
            };
        }
    }

        [GlobalCleanup]
    public void GlobalCleanup()
    {
        _dbContext.Dispose();
    }

}


