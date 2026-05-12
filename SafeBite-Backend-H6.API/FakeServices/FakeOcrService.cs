namespace SafeBite_Backend_H6.API.FakeServices;

public class FakeOcrService : IOcrService
{
    public Task<OcrResponseDto> ExtractTextFromImageAsync(
        Stream imageStream,
        string languages = null)
    {
        var response = new OcrResponseDto
        {
            Language = languages ?? "eng",
            Status = "success",
            IngredientsText = "Ingredients: Sugar, Peanuts, Cocoa Butter"
        };

        return Task.FromResult(response);
    }
}
