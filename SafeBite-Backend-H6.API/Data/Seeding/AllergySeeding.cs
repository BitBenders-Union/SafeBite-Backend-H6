namespace SafeBite_Backend_H6.API.Data.Seeding;

public class AllergySeeding
{
    public static async Task SeedAllergiesAsync(AppDbContext context)
    {
        if (await context.Allergies.AnyAsync())
        {
            return;
        }

        await context.Allergies.AddRangeAsync(Allergies.All);
        await context.SaveChangesAsync();
    }

}
