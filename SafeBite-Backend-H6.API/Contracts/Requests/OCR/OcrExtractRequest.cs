namespace SafeBite_Backend_H6.API.Contracts.Requests.OCR;

public class OcrExtractRequest
{
    public IFormFile Image { get; set; } = null!;
    public string? Lang { get; set; }
}
