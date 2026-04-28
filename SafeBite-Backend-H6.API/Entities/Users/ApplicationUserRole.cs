namespace SafeBite_Backend_H6.API.Entities.Users;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public ApplicationUser User { get; set; }
    public IdentityRole Role { get; set; }
}
