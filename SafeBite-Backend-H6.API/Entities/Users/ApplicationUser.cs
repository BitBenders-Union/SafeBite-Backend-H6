namespace SafeBite_Backend_H6.API.Entities.Users;

public class ApplicationUser : IdentityUser
{
    public bool IsDeactivated { get; set; } = false;
    public DateTime DeactivatedTime { get; set; }

}
