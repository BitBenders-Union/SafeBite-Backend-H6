namespace SafeBite_Backend_H6.API.Services.Scans;

public class ScanService : IScanService
{
    private readonly IScanRepository _scanRepository;
    private readonly IOcrService _ocrService;
    private readonly IUserAllergyAnalysisService _userAllergyAnalysisService;
    private readonly IScanAnalysisService _scanAnalysisService;

    public ScanService(
        IScanRepository scanRepository,
        IOcrService ocrService,
        IUserAllergyAnalysisService userAllergyAnalysisService,
        IScanAnalysisService scanAnalysisService)
    {
        _scanRepository = scanRepository;
        _ocrService = ocrService;
        _userAllergyAnalysisService = userAllergyAnalysisService;
        _scanAnalysisService = scanAnalysisService;
    }

    public async Task<ScanResponse> CreateAsync(string userId, CreateScanRequest request)
    {
        // validation
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        if (request.Image is null || request.Image.Length == 0)
            throw new ArgumentException("Image is required.");

        // process image with OCR
        using var stream = request.Image.OpenReadStream();

        OcrResponseDto ocrResult = await _ocrService.ExtractTextFromImageAsync(stream, request.Lang);

        if (string.IsNullOrWhiteSpace(ocrResult.IngredientsText))
            throw new InvalidOperationException("No ingredients text could be extracted from the image.");

        // get user allergies and Custom User Allergiers
        var userAllergies = await _userAllergyAnalysisService.GetAllAllergiesForUserAsync(userId);

        // map to analysis request
        var analysisRequest = new ScanAnalysisRequest
        {
            IngredientsText = ocrResult.IngredientsText,
            Allergies = userAllergies
        };

        var analysisResult = await _scanAnalysisService.AnalyzeIngredientsAsync(analysisRequest);

        Scan scan = ScanMappings.ToEntity(userId, request.Name, analysisResult);

        await _scanRepository.AddAsync(scan);
        await _scanRepository.SaveChangesAsync();

        var createdScan = await _scanRepository.GetFullScanByIdAsync(scan.Id);

        if (createdScan is null)
            throw new KeyNotFoundException("Created scan could not be reloaded.");

        return ScanMappings.ToResponse(createdScan);
    }

    public async Task<PagedResult<ScanResponse>> GetPagedByUserIdAsync(string userId, PaginationParameters parameters, string? searchTerm = null, bool? hasDetectedAllergies = null)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        var query = _scanRepository.QueryByUserId(userId, searchTerm, hasDetectedAllergies);

        PagedResult<Scan> result = await _scanRepository.GetPagedAsync(parameters, query);

        return result.Map(ScanMappings.ToResponse);
    }

    public async Task<ScanResponse?> GetByIdAsync(string userId, Guid scanId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        if (scanId == Guid.Empty)
            throw new ArgumentException("Scan id cannot be empty.");

        var scan = await _scanRepository.GetFullScanByIdAsync(scanId, userId);

        if (scan is null)
            return null;

        return ScanMappings.ToResponse(scan);
    }

    public async Task<bool> DeleteAsync(string userId, Guid scanId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        if (scanId == Guid.Empty)
            throw new ArgumentException("Scan id cannot be empty.");

        var scan = await _scanRepository.GetByIdAsync(scanId, userId);

        if (scan is null)
            return false;

        bool deleted = await _scanRepository.DeleteAsync(scanId);

        if (deleted)
            await _scanRepository.SaveChangesAsync();

        return deleted;
    }
}
