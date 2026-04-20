namespace SafeBite_Backend_H6.API.Entities.Scans;

public class Scan
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTime ScannedAt { get; set; }
    public ICollection<ScanDetectedAllergies> DetectedAllergies { get; set; } = [];
}
