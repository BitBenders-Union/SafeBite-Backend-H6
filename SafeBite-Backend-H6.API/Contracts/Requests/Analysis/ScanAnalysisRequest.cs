namespace SafeBite_Backend_H6.API.Contracts.Requests.Analysis;

public class ScanAnalysisRequest
{
    public string IngredientsText { get; set; } = string.Empty;
    public List<AllergyAnalysisItem> Allergies { get; set; } = [];
}
