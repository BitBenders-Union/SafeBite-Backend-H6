namespace SafeBite_Backend_H6.API.Contracts.AiAnalysisDTOs;

public class DetectedAllergyAiItem
{
    [JsonPropertyName("allergy_id")]
    public string? AllergyId { get; set; }

    [JsonPropertyName("allergy_name")]
    public string? AllergyName { get; set; }

    [JsonPropertyName("matched_ingredients")]
    public List<string>? MatchedIngredients { get; set; }
}
