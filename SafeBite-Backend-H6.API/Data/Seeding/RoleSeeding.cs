namespace SafeBite_Backend_H6.API.Data.Seeding;

public class RoleSeeding
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = Roles.AllRoles;

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
