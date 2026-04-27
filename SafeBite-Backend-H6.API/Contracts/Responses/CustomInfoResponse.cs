namespace SafeBite_Backend_H6.API.Contracts.Responses;

public class CustomInfoResponse
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required bool IsEmailConfirmed { get; set; }
    public required IList<string> Roles { get; set; }
    public required bool IsDeactivated { get; set; }
    public DateTime? DeactivatedTime { get; set; }
}
