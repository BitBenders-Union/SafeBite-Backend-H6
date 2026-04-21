namespace SafeBite_Backend_H6.API.Contracts.Responses.Analysis;

public class ScanAnalysisResult
{
    public string IngredientsText { get; set; } = string.Empty;
    public List<DetectedAllergyAnalysisResult> DetectedAllergies { get; set; } = [];
}
