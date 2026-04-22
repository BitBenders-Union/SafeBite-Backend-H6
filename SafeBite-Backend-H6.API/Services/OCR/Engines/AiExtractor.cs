

using SafeBiteApi.Utilities.Constants;

namespace SafeBiteApi.Services.OCR.Engines;

public class AiExtractor : IAiExtractor
{
    private readonly ChatClient _chatTextClient;
    private readonly ChatClient _chatVisionClient;

    public AiExtractor(IConfiguration config)
    {
        //This collect the Apikey from user secrets
        var apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        _chatTextClient = new ChatClient("gpt-4o-mini", apiKey);
        _chatVisionClient = new ChatClient("gpt-4o", apiKey);
    }

    // The method under here is used if the tesseract extract a string of text to parse it into Json
    public async Task<string> ExtractIngredientsFromTextAsync(string rawText)
    {
        var systemMsg = new SystemChatMessage(PromptConstants.IngredientExtractionSystemPrompt);

        var userMsg = new UserChatMessage($"OCR text:\n\n{rawText}");
        var completion = await _chatTextClient.CompleteChatAsync(new ChatMessage[] { systemMsg, userMsg });

        return completion.Value.Content.FirstOrDefault()?.Text?.Trim() ?? "";
    }

    // The method under here is the Fallback method that we call if the Tesseract could not read the image
    public async Task<OcrResponseDto> FallbackWithVisionAsync(byte[] imageBytes, string languages)
    {
        var systemMsg = new SystemChatMessage(PromptConstants.VisionFallbackSystemPrompt);

        var userMsg = new UserChatMessage(
            ChatMessageContentPart.CreateTextPart("Extract only the ingredients list."),
            ChatMessageContentPart.CreateImagePart(new BinaryData(imageBytes), "image/png")
        );

        var completion = await _chatVisionClient.CompleteChatAsync(new ChatMessage[] { systemMsg, userMsg });
        var raw = completion.Value.Content.FirstOrDefault()?.Text?.Trim() ?? "";

        return ParseAiJson(raw, languages);
    }
    public OcrResponseDto ParseAiJson(string input, string languages)
    {
        // We search for everything between the first and last curly brace
        var jsonMatch = Regex.Match(input, @"\{.*\}", RegexOptions.Singleline);

        // If we dont find anything that looks like JSON, we return "unreadable".
        if (!jsonMatch.Success) return new OcrResponseDto { Status = "unreadable", Language = languages };

        var json = jsonMatch.Value;

        // Look for the word "status" and grab whatever text is inside the quotes.
        var status = Regex.Match(json, @"""status""\s*:\s*""(.*?)""").Groups[1].Value;

        // Do the exact same thing for "ingredients_text"
        var ingredients = Regex.Match(json, @"""ingredients_text""\s*:\s*""(.*?)""").Groups[1].Value;

        return new OcrResponseDto
        {
            Language = languages,
            // If the status field came back empty, default it to "unreadable".
            Status = string.IsNullOrEmpty(status) ? "unreadable" : status,

            //Removing extra whitespace at the ends.
            IngredientsText = ingredients?.Replace("\n", " ").Trim() ?? ""
        };
    }
}
