namespace SafeBite_Backend_H6.API.Data.Seeding;

public class UserSeeding
{
    public static async Task SeedAdminAsync(
           UserManager<ApplicationUser> userManager,
           IConfiguration config)
    {
        var adminEmail = config["Seed:AdminEmail"];
        var adminPassword = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new Exception("Admin password not configured in user secrets");
        
        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new Exception("Admin email not configured in user secrets");

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
