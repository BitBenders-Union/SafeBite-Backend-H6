

namespace SafeBite_Backend_H6.API.Data.IdentityDb;

// Added identity claims and roles so we can use navigation property to querry data using EF. It shouldn't change Migration at all
public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string, IdentityUserClaim<string>, ApplicationUserRole,IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            entity.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
}
