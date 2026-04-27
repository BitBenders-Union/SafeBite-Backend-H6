namespace SafeBite_Backend_H6.API.Mappings;

public class CustomInfoMappings
{
    public static async Task<CustomInfoResponse> ToResponseAsync(
          ApplicationUser user,
          UserManager<ApplicationUser> userManager)
    {
        var userId = await userManager.GetUserIdAsync(user)
            ?? throw new NotSupportedException("UserId could not be found.");

        var email = await userManager.GetEmailAsync(user)
            ?? throw new NotSupportedException("User must have an email.");

        var roles = await userManager.GetRolesAsync(user);
        var isEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

        return new CustomInfoResponse
        {
            UserId = userId,
            Email = email,
            IsEmailConfirmed = isEmailConfirmed,
            Roles = roles,
            IsDeactivated = user.IsDeactivated,
            DeactivatedTime = user.IsDeactivated ? user.DeactivatedTime : null
        };
    }
}
