using SafeBite_Backend_H6.API.Interfaces.Services.OCR;
using SafeBite_Backend_H6.API.Repositories;
using SafeBite_Backend_H6.API.Shared;
using SafeBite_Backend_H6.API.Mappings;
using SafeBiteApi.Utilities;

namespace SafeBite_Backend_H6.API.Services.Scans;

public class ScanService : IScanService
{
    private readonly IScanRepository _scanRepository;
    private readonly IOcrService _ocrService;
    private readonly IUserAllergyAnalysisService _userAllergyAnalysisService;
    private readonly IScanAnalysisService _scanAnalysisService;
    private readonly IAllergyMatcher _allergyMatcher;

    public ScanService(
        IScanRepository scanRepository,
        IOcrService ocrService,
        IUserAllergyAnalysisService userAllergyAnalysisService,
        IScanAnalysisService scanAnalysisService,
        IAllergyMatcher allergyMatcher)
    {
        _scanRepository = scanRepository;
        _ocrService = ocrService;
        _userAllergyAnalysisService = userAllergyAnalysisService;
        _scanAnalysisService = scanAnalysisService;
        _allergyMatcher = allergyMatcher;
    }

    public async Task<ScanResponse> CreateAsync(string userId, CreateScanRequest request)
    {
        // 1. Validering
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        if (request.Image is null || request.Image.Length == 0)
            throw new ArgumentException("Image is required.");

        // 2. OCR 
        using var stream = request.Image.OpenReadStream();
        OcrResponseDto ocrResult = await _ocrService.ExtractTextFromImageAsync(stream, request.Lang);

        if (string.IsNullOrWhiteSpace(ocrResult.IngredientsText))
            throw new InvalidOperationException("No ingredients text could be extracted from the image.");

        // 3. Hent brugerens allergier også custom
        var userAllergies = await _userAllergyAnalysisService.GetAllAllergiesForUserAsync(userId);

        // 4. DETERMINISTIC MATCH Via hjælpeklasse
        var localMatches = _allergyMatcher.MatchLocalAllergies(ocrResult.IngredientsText, userAllergies);

        // 5. AI ANALYSE (OpenAI)
        var analysisRequest = new ScanAnalysisRequest
        {
            IngredientsText = ocrResult.IngredientsText,
            Allergies = userAllergies
        };

        var analysisResult = await _scanAnalysisService.AnalyzeIngredientsAsync(analysisRequest);

        // 6. MERGE ved hjælp fra hjælpeklassen til at samle fund fra AI og Determistic
        _allergyMatcher.MergeResults(analysisResult, localMatches);

        // 7. Gem i databasen
        Scan scan = ScanMappings.ToEntity(userId, request.Name, analysisResult);

        await _scanRepository.AddAsync(scan);
        await _scanRepository.SaveChangesAsync();

        // 8. Hent den fulde scan
        var createdScan = await _scanRepository.GetFullScanByIdAsync(scan.Id);

        if (createdScan is null)
            throw new KeyNotFoundException("Created scan could not be reloaded.");

        return ScanMappings.ToResponse(createdScan);
    }

    //Standard Service Metoder

    public async Task<PagedResult<ScanResponse>> GetPagedByUserIdAsync(string userId, PaginationParameters parameters, string? searchTerm = null, bool? hasDetectedAllergies = null)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var query = _scanRepository.QueryByUserId(userId, searchTerm, hasDetectedAllergies);
        PagedResult<Scan> result = await _scanRepository.GetPagedAsync(parameters, query);
        return result.Map(ScanMappings.ToResponse);
    }

    public async Task<ScanResponse?> GetByIdAsync(string userId, Guid scanId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var scan = await _scanRepository.GetFullScanByIdAsync(scanId, userId);
        return scan is null ? null : ScanMappings.ToResponse(scan);
    }

    public async Task<bool> DeleteAsync(string userId, Guid scanId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var scan = await _scanRepository.GetByIdAsync(scanId, userId);
        if (scan is null) return false;

        bool deleted = await _scanRepository.DeleteAsync(scanId);
        if (deleted) await _scanRepository.SaveChangesAsync();
        return deleted;
    }
}
