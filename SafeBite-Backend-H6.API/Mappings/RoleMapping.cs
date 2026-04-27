namespace SafeBite_Backend_H6.API.Mappings;

public class RoleMapping
{
    public static RoleResponse ToResponse(IdentityRole role)
    {
        return new RoleResponse
        {
            RoleId = role.Id,
            RoleName = role.Name
        };
    }
}
