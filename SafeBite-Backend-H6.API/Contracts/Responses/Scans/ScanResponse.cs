namespace SafeBite_Backend_H6.API.Contracts.Responses.Scans;

public class ScanResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTime ScannedAt { get; set; }
    public string ScannedIngredientsText { get; set; } = string.Empty;
    public List<ScanDetectedAllergyResponse> DetectedAllergies { get; set; } = [];
}
