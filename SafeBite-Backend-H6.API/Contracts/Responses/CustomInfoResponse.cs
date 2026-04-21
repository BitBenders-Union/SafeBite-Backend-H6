namespace SafeBite_Backend_H6.API.Contracts.Responses;

public class CustomInfoResponse
{
    public required string Email { get; init; }
    public required bool IsEmailConfirmed { get; init; }
    public required string UserId { get; init; }
    public required IList<string> Roles { get; init; }
}
