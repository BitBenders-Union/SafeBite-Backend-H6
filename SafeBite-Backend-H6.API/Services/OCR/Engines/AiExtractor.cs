

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
        var systemMsg = new SystemChatMessage("""
You are an expert at interpreting OCR output from food labels in Danish, Swedish, Norwegian, French, and English.
You will receive noisy OCR text with possible errors.

Your job:
1. Detect if an ingredients list exists.
2. Clean and normalize it without guessing.
3. Return ONLY valid JSON:
{
  "status": "complete" | "unreadable",
  "status": "complete" | "unreadable",
  "ingredients_text": "<text>"
}
""");

        var userMsg = new UserChatMessage($"OCR text:\n\n{rawText}");
        var completion = await _chatTextClient.CompleteChatAsync(new ChatMessage[] { systemMsg, userMsg });

        return completion.Value.Content.FirstOrDefault()?.Text?.Trim() ?? "";
    }

    // The method under here is the Fallback method that we call if the Tesseract could not read the image
    public async Task<OcrResponseDto> FallbackWithVisionAsync(byte[] imageBytes, string languages)
    {
        var systemMsg = new SystemChatMessage("""
You receive a food label photo.

Your job:
1. Read the ENTIRE visible text block related to ingredients.
   Do NOT stop after the first line.
   Include all continuation lines, including allergy warnings.

2. Extract ONLY:
   - Ingredient list text
   - All allergen warning lines such as:
     "may contain ...",
     "can contain ...",
     "spor af ...",
     "may contain traces of ...",
     "kann spuren von ...",
     "kan inneholde spor av ..."

3. From any warning line, extract ONLY the FIRST allergen term for each allergen group.
   Example:
   "nødder/nötter/nuts" → extract ONLY "nødder".

4. Append the extracted allergens to the end of ingredients_text, separated by commas.

STRICT RULES:
- Do NOT translate or rewrite ingredients.
- Do NOT generate synonyms.
- Do NOT output multiple languages for the same allergen.
- Extract allergen words EXACTLY as printed, but only the FIRST variant.

OUTPUT STRICT JSON ONLY:

{
  "status": "complete" | "unreadable",
  "ingredients_text": "<text>"
}
""");

        var userMsg = new UserChatMessage(
            ChatMessageContentPart.CreateTextPart("Extract only the ingredients list."),
            ChatMessageContentPart.CreateImagePart(new BinaryData(imageBytes), "image/png")
        );

        var completion = await _chatVisionClient.CompleteChatAsync(new ChatMessage[] { systemMsg, userMsg });
        var raw = completion.Value.Content.FirstOrDefault()?.Text?.Trim() ?? "";

        return ParseAiJson(raw, languages);
    }

    // The method under here is to parse the AI response to a Json format
    public OcrResponseDto ParseAiJson(string input, string languages)
    {
        // We search for everything between the first and last curly brace
        // Singleline mode ensures we catch the text even if the AI included line breaks.
        var jsonMatch = Regex.Match(input, @"\{.*\}", RegexOptions.Singleline);

        // If we don't find anything that looks like JSON, we return "unreadable".
        if (!jsonMatch.Success) return new OcrResponseDto { Status = "unreadable", Language = languages };

        var json = jsonMatch.Value;

        // Look for the word "status" and grab whatever text is inside the quotes.
        // \s* means we don't care if the AI added extra spaces around.
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
