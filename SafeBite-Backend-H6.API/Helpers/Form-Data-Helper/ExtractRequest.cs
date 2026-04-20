namespace SafeBite_Backend_H6.API.Helpers.Form_Data_Helper;

public sealed class ExtractRequest
{
    public IFormFile Image { get; set; } = default!;
    public string? Lang { get; set; }
}
