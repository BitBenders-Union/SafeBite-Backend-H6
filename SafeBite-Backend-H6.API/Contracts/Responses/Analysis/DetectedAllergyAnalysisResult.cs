namespace SafeBite_Backend_H6.API.Contracts.Responses.Analysis;

public class DetectedAllergyAnalysisResult
{
    public Guid AllergyId { get; set; }
    public string AllergyName { get; set; } = string.Empty;
    public AllergyType AllergyType { get; set; }
    public List<string> MatchedIngredients { get; set; } = [];
}
