namespace SafeBite_Backend_H6.API.Contracts.Requests.Scans;

public class CreateScanRequest
{
    public IFormFile Image { get; set; } = null!;
    public string? Name { get; set; }
    public string Lang { get; set; }
}
