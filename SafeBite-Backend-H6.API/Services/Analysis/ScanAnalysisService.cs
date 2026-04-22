using OpenAI.Chat;
using SafeBiteApi.Utilities.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SafeBiteApi.Services.Analysis;

public class ScanAnalysisService : IScanAnalysisService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<ScanAnalysisService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new();

    public ScanAnalysisService(IConfiguration config, ILogger<ScanAnalysisService> logger)
    {
        var apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        _chatClient = new ChatClient("gpt-4o-mini", apiKey);
        _logger = logger;
    }

    public async Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request)
    {
        // Validering
        if (string.IsNullOrWhiteSpace(request.IngredientsText) || request.Allergies == null || !request.Allergies.Any())
        {
            return new ScanAnalysisResult { IngredientsText = request.IngredientsText?.Trim() ?? "", DetectedAllergies = [] };
        }

        // 1. Byg beskederne ved hjælp af PromptConstants
        var messages = new ChatMessage[] {
            new SystemChatMessage(PromptConstants.ScanAnalysisSystemPrompt),
            new UserChatMessage(PromptConstants.BuildScanUserPrompt(request.IngredientsText, request.Allergies))
        };

        try
        {
            // 2. Kald OpenAI
            var completion = await _chatClient.CompleteChatAsync(messages);
            var rawResponse = completion.Value.Content.FirstOrDefault()?.Text?.Trim() ?? "";

            return ProcessAiResponse(rawResponse, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI scan analysis request failed.");
            throw;
        }
    }

    private ScanAnalysisResult ProcessAiResponse(string raw, ScanAnalysisRequest request)
    {
        var json = ExtractJson(raw);
        if (string.IsNullOrWhiteSpace(json)) return CreateEmptyResult(request);

        try
        {
            var parsed = JsonSerializer.Deserialize<ScanAnalysisAiResponse>(json, _jsonOptions);
            if (parsed == null) return CreateEmptyResult(request);

            var allowedAllergies = request.Allergies.ToDictionary(allergies => allergies.AllergyId, allergies => allergies);
            var detected = new List<DetectedAllergyAnalysisResult>();

            foreach (var item in parsed.DetectedAllergies ?? [])
            {
                if (Guid.TryParse(item.AllergyId, out var allergyId) && allowedAllergies.TryGetValue(allergyId, out var allowedAllergy))
                {
                    detected.Add(new DetectedAllergyAnalysisResult
                    {
                        AllergyId = allergyId,
                        AllergyName = allowedAllergy.AllergyName,
                        MatchedIngredients = item.MatchedIngredients?.Where(ingredient => !string.IsNullOrWhiteSpace(ingredient)).Select(ingredient => ingredient.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? []
                    });
                }
            }

            return new ScanAnalysisResult
            {
                IngredientsText = string.IsNullOrWhiteSpace(parsed.IngredientsText)
                ? request.IngredientsText.Trim()
                : parsed.IngredientsText.Trim(),
                DetectedAllergies = detected
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse AI response.");
            return CreateEmptyResult(request);
        }
    }

    private string ExtractJson(string input)
    {
        var firstBracket = input.IndexOf('{');
        var lastBracket = input.LastIndexOf('}');

        if (firstBracket < 0 || lastBracket <= firstBracket)
            return string.Empty;

        return input[firstBracket..(lastBracket + 1)];
    }

    private ScanAnalysisResult CreateEmptyResult(ScanAnalysisRequest request) =>
        new() { IngredientsText = request.IngredientsText.Trim(), DetectedAllergies = [] };

    private sealed class ScanAnalysisAiResponse
    {
        [JsonPropertyName("ingredients_text")]
        public string? IngredientsText { get; set; }

        [JsonPropertyName("detected_allergies")]
        public List<DetectedAllergyAiItem>? DetectedAllergies { get; set; }
    }

    private sealed class DetectedAllergyAiItem
    {
        [JsonPropertyName("allergy_id")]
        public string? AllergyId { get; set; }

        [JsonPropertyName("allergy_name")]
        public string? AllergyName { get; set; }

        [JsonPropertyName("matched_ingredients")]
        public List<string>? MatchedIngredients { get; set; }
    }
}
