namespace SafeBite_Backend_H6.API.Contracts.Requests.OCR;

public class AiAllergyAnalysis
{
    public string OverallRisk { get; set; } = "none";

    public List<string> MatchedAllergy { get; set; } = new();

    public List<string> MatchedCrossAllergy { get; set; } = new();

    public List<string> FlaggedIngredients { get; set; } = new();

    public List<string> Inputs_Ingredients { get; set; } = new();

    public List<string> Inputs_Allergies { get; set; } = new();
}
