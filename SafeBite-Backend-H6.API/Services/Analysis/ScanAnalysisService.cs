using System.Text.Json;
using System.Text.Json.Serialization;

namespace SafeBite_Backend_H6.API.Services.Analysis;

public class ScanAnalysisService : IScanAnalysisService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<ScanAnalysisService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true // meaning: it doesn't care for upper or lower case when comparing to the properies in ScanAnalysisAiResponse.
                                           // it does not help with snake_case, so we are forced to add json serilization rules to the ScanAnalysisAiResponse class.
    };

    public ScanAnalysisService(IConfiguration config, ILogger<ScanAnalysisService> logger)
    {
        var apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI API key is not configured.");

        _chatClient = new ChatClient("gpt-4o-mini", apiKey);
        _logger = logger;
    }

    public async Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request)
    {
        // validation
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.IngredientsText))
            throw new ArgumentException("IngredientsText cannot be empty.");

        // early return
        if (request.Allergies is null || request.Allergies.Count == 0)
        {
            _logger.LogInformation("Scan analysis skipped because user has no allergies.");
            return new ScanAnalysisResult
            {
                IngredientsText = request.IngredientsText.Trim(),
                DetectedAllergies = []
            };
        }

        // logging input data
        _logger.LogInformation("User allergies count: {Count}", request.Allergies.Count);
        _logger.LogDebug("Ingredients text: {IngredientsText}", request.IngredientsText);

        foreach (var allergy in request.Allergies)
        {
            _logger.LogInformation("User allergy: {AllergyId} - {AllergyName}", allergy.AllergyId, allergy.AllergyName);
        }

        // builds the system and user messages for the chat completion request
        var systemMsg = BuildSystemMessage();
        var userMsg = BuildUserMessage(request);

        ChatCompletion completion; // declaring variable so it can be used in this scope for logging after the try-catch block

        try
        {
            // sends the prompt for chat analysis
            completion = await _chatClient.CompleteChatAsync([systemMsg, userMsg]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI scan analysis request failed.");
            throw;
        }

        // extracts the text from ai response
        var raw = completion.Content.FirstOrDefault()?.Text?.Trim() ?? string.Empty;

        // converts the extracted text into the ScanAnalysisResult object
        var result = ParseAiJson(raw, request);
        _logger.LogInformation("Scan analysis result. Detected allergies count: {Count}", result.DetectedAllergies.Count);

        // more logging
        foreach (var detected in result.DetectedAllergies)
        {
            _logger.LogInformation("Detected allergy: {AllergyId} - {AllergyName}", detected.AllergyId, detected.AllergyName);

            foreach (var match in detected.MatchedIngredients)
            {
                _logger.LogInformation("Matched ingredient: {MatchedIngredient}", match);
            }
        }

        return result;
    }

    private ScanAnalysisResult ParseAiJson(string input, ScanAnalysisRequest request)
    {
        // early return
        if (string.IsNullOrWhiteSpace(input))
        {
            _logger.LogWarning("AI response was empty.");
            return new ScanAnalysisResult
            {
                IngredientsText = request.IngredientsText.Trim(),
                DetectedAllergies = []
            };
        }

        // gets json part of the input string.
        var json = ExtractJson(input);

        // early return if ai response does not contain valid json
        if (string.IsNullOrWhiteSpace(json))
        {
            _logger.LogWarning("Could not extract JSON from AI response. AI response: {RawResponse}", input);
            return new ScanAnalysisResult
            {
                IngredientsText = request.IngredientsText.Trim(),
                DetectedAllergies = []
            };
        }

        _logger.LogDebug("Extracted JSON from AI response: {Json}", json);

        try
        {
            // tries mapping the json to the ScanAnalysisAiResponse object
            var parsed = JsonSerializer.Deserialize<ScanAnalysisAiResponse>(json, _jsonOptions);

            _logger.LogDebug("Parsed ingredients text: {IngredientsText}", parsed?.IngredientsText);
            _logger.LogDebug("Parsed detected allergies null? {IsNull}", parsed?.DetectedAllergies is null);

            // early return if deserialization fails and returns null / doesn't match scanAnalysisAiResponse
            if (parsed is null)
            {
                _logger.LogWarning("Deserialized AI response was null.");
                return new ScanAnalysisResult
                {
                    IngredientsText = request.IngredientsText.Trim(),
                    DetectedAllergies = []
                };
            }

            // transform allergy list to a dictionary for easy lookup
            var allowedAllergies = request.Allergies.ToDictionary(a => a.AllergyId, a => a);

            // prepare list for detected allergies, accessible in this scope.
            var detected = new List<DetectedAllergyAnalysisResult>();

            // if DetectedAllergies is null, it will just skip the loop since we create an empty array instead.
            // you could also create an if statement but i think this looks cleaner :^) - jonas
            foreach (var item in parsed.DetectedAllergies ?? [])
            {
                // validation - ensures we only use proper ai response items.
                // remember continue starts the loop again for the next item.
                if (string.IsNullOrWhiteSpace(item.AllergyId))
                {
                    _logger.LogWarning("Skipped detected allergy because allergy_id was empty.");
                    continue;
                }

                if (!Guid.TryParse(item.AllergyId, out var allergyId)) // tries converting string to guid. if it fails ai response was invalid
                {
                    _logger.LogWarning("Skipped detected allergy because allergy_id was not a valid Guid. Value: {AllergyId}", item.AllergyId);
                    continue;
                }

                if (!allowedAllergies.TryGetValue(allergyId, out var allowedAllergy)) // uses the dictionary to check if the allergyId (from above guid.tryparse) matches the user's allowed allergy by id.
                {
                    _logger.LogWarning("Skipped detected allergy because allergy_id was not in allowed allergy list. Value: {AllergyId}", allergyId);
                    continue;
                }

                // the following can be compressed to one line but for better readability i split them up.

                // extracts matched ingredients. if null replaced by empty array.
                var ingredients = item.MatchedIngredients ?? [];

                // remove empty values and trim
                var cleaned = ingredients
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim());

                // removes duplicates of the cleaned list. OrdinalIgnoreCase compares string values like "mælk" and "Mælk" as equals without the use of either toLower() or toUpper().
                // https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase?view=net-10.0 - see remarks
                // https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings is also mentioned here for recommendations on how to compare strings.
                var distinct = cleaned
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                var matches = distinct.ToList(); // converts to list

                // mapping
                detected.Add(new DetectedAllergyAnalysisResult
                {
                    AllergyId = allergyId,
                    AllergyName = allowedAllergy.AllergyName,
                    MatchedIngredients = matches
                });
            }

            // return final mapped result
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
            _logger.LogError(ex, "Failed to parse AI response. Raw input: {RawResponse}", input);

            // return empty on fail
            return new ScanAnalysisResult
            {
                IngredientsText = request.IngredientsText.Trim(),
                DetectedAllergies = []
            };
        }
    }

    private static SystemChatMessage BuildSystemMessage()
    {
        return new SystemChatMessage("""
You are an assistant that analyzes food ingredient text against a user-specific allergy list.

You will receive:
1. A food ingredients text
2. A list of allergies that belong to the user

Your task:
- Return ONLY allergies from the provided allergy list
- Detect an allergy if it is present either:
  1. explicitly by name in the ingredient text, or
  2. implicitly through ingredients that are well-known sources of that allergen
- For each detected allergy, return the exact ingredient word or phrase from the ingredient text that caused the match
- matched_ingredients must contain only words or phrases that actually appear in the ingredient text
- Do not invent allergies that are not in the provided list
- Do not invent ingredient phrases that are not present in the text
- Do not return duplicate allergies
- If nothing is detected, return an empty array

Examples:
- If the allergy is Gluten, ingredients such as wheat flour, rye flour, barley malt, wheat, or wheat flour may count as a match
- If the allergy is Milk, ingredients such as yoghurt, whey, cream, milk powder, cheese, or milk may count as a match
- If the allergy is Egg, ingredients such as egg white, egg yolk, egg, or albumin may count as a match

Return STRICT JSON only in this format:
{
  "ingredients_text": "<original or cleaned ingredients text>",
  "detected_allergies": [
    {
      "allergy_id": "<guid from provided list>",
      "allergy_name": "<name from provided list>",
      "matched_ingredients": ["<exact ingredient text from input>"]
    }
  ]
}
""");
    }

    private static UserChatMessage BuildUserMessage(ScanAnalysisRequest request)
    {
        // formats allergies
        var allergyLines = request.Allergies
            .Select(a => $"- allergy_id: {a.AllergyId}, allergy_name: {a.AllergyName}");

        // converts the list of allergies into a single line seperated string.
        var allergyText = string.Join(Environment.NewLine, allergyLines);

        return new UserChatMessage($$"""
Ingredients text:
{{request.IngredientsText}}

User allergy list:
{{allergyText}}
""");
    }

    private static string ExtractJson(string input)
    {

        // tries to find json structure in a string text
        var firstBracket = input.IndexOf('{');
        var lastBracket = input.LastIndexOf('}');

        if (firstBracket < 0 || lastBracket <= firstBracket)
            return string.Empty;

        // returns everything between first "{" and last "}" (including the Brackets)
        return input[firstBracket..(lastBracket + 1)];
    }

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
