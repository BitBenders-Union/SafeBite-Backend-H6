using SafeBite_Backend_H6.API.Contracts.Responses.OCR;
namespace SafeBite_Backend_H6.API.Interfaces.Services.OCR;

public interface IAiExtractor
{
    Task<string> ExtractIngredientsFromTextAsync(string rawText);
    Task<OcrResponseDto> FallbackWithVisionAsync(byte[] imageBytes, string languages);
    OcrResponseDto ParseAiJson(string input, string languages);
}
