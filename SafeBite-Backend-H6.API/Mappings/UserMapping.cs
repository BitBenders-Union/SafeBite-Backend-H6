namespace SafeBite_Backend_H6.API.Mappings;

public class UserMapping
{
    public static UserResponse ToResponse(ApplicationUser user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            IsActive = !user.IsDeactivated,
            IsLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow,
            Roles = user.UserRoles.Select(ur => new RoleResponse
            {
                RoleId = ur.Role.Id,
                RoleName = ur.Role.Name!
            })
            .ToList()
        };
    }
}
