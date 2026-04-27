namespace SafeBite_Backend_H6.API.Contracts.Requests.Role;

public class AssignRoleRequest
{
    public required string UserID { get; set; }
    public required string RoleName { get; set; }
}
