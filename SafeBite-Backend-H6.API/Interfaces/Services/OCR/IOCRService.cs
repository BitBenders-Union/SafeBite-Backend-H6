using SafeBite_Backend_H6.API.Contracts.Responses.OCR;

namespace SafeBite_Backend_H6.API.Interfaces.Services.OCR;

public interface IOcrService
{
    /// <summary>
    /// Orchestrates the full OCR flow: Preprocessing, Tesseract, and AI refinement.
    /// </summary>
    Task<OcrResponseDto> ExtractTextFromImageAsync(Stream imageStream, string languages = null);
}
