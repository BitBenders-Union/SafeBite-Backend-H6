namespace SafeBite_Backend_H6.API.Contracts.AiAnalysisDTOs;

public class ScanAnalysisAiResponse
{
    [JsonPropertyName("ingredients_text")]
    public string? IngredientsText { get; set; }

    [JsonPropertyName("detected_allergies")]
    public List<DetectedAllergyAiItem>? DetectedAllergies { get; set; }
}
