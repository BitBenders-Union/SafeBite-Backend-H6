namespace SafeBite_Backend_H6.API.Services.OCR;

public class OcrService : IOcrService
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IAiExtractor _aiExtractor;
    private readonly string _tessDataPath;
    private readonly string _defaultLangs = "eng+dan+swe+nor+fra";
    private const float ConfidenceThreshold = 0.75f;

    public OcrService(IImageProcessor imageProcessor, IAiExtractor aiExtractor)
    {
        _imageProcessor = imageProcessor;
        _aiExtractor = aiExtractor;

        // Tell Tesseract where to find the language training data
        _tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
    }

    public async Task<OcrResponseDto> ExtractTextFromImageAsync(Stream imageStream, string languages = null)
    {
        languages ??= _defaultLangs;

        var processedBytes = _imageProcessor.PreprocessForOcr(imageStream);
        var ocrResult = RunTesseract(processedBytes, languages);

        // If the text is readable and confidence is high, we let the AI clean up the text.
        if (ocrResult != null && IsTextDecent(ocrResult.Text) && ocrResult.Confidence >= ConfidenceThreshold)
        {
            var cleanedJson = await _aiExtractor.ExtractIngredientsFromTextAsync(ocrResult.Text);

            return _aiExtractor.ParseAiJson(cleanedJson, languages);
        }

        //If Tesseract failed or the text was gibberish, we use GPT-4o Vision as a fallback
        return await _aiExtractor.FallbackWithVisionAsync(processedBytes, languages);
    }

    private OcrResultDto RunTesseract(byte[] imageBytes, string lang)
    {
        try
        {
            using var pix = Pix.LoadFromMemory(imageBytes);
            using var engine = new TesseractEngine(_tessDataPath, lang, EngineMode.Default);
            using var page = engine.Process(pix, PageSegMode.SingleBlock);

            return new OcrResultDto
            {
                Text = page.GetText().Trim(),
                Confidence = page.GetMeanConfidence()
            };
        }
        catch
        {
            // If Tesseract crashes, it return null to trigger fallback
            return null;
        }
    }

    private bool IsTextDecent(string text)
    {
        // Basic check: If its too short, its probably not an ingredients list
        if (string.IsNullOrWhiteSpace(text) || text.Length < 40) return false;

        // Check if it contains common keywords for ingredients
        var keywords = new[] { "ingrediens", "ingredients", "zutaten", "innehåll" };
        return keywords.Any(keywords => text.Contains(keywords, StringComparison.OrdinalIgnoreCase));
    }
}
