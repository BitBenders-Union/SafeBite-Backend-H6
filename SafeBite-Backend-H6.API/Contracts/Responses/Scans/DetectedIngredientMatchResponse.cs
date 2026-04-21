namespace SafeBite_Backend_H6.API.Contracts.Responses.Scans;

public class DetectedIngredientMatchResponse
{
    public Guid Id { get; set; }
    public string IngredientText { get; set; } = null!;
}
