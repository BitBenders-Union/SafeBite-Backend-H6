

namespace SafeBite_Backend_H6.API.Data.AppDb;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    // Define dbsets here
    public DbSet<Allergy> Allergies => Set<Allergy>();
    public DbSet<AllergyUser> AllergyUsers => Set<AllergyUser>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<ScanDetectedAllergies> ScanDetectedAllergies => Set<ScanDetectedAllergies>();
    public DbSet<DetectedIngredientMatch> DetectedIngredientMatches => Set<DetectedIngredientMatch>();
    public DbSet<CustomAllergy> CustomAllergies => Set<CustomAllergy>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure entity relationships and constraints here

        modelBuilder.Entity<Allergy>(entity =>
        {
            entity.HasKey(a => a.Id); // sætter id som primary key

            entity.Property(a => a.Name) // tvinger name til at være required og max lenght på 100 tegn
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.NormalizedName) 
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(a => a.NormalizedName) // laver Name unique og sætter index
                .IsUnique();
        });


        modelBuilder.Entity<AllergyUser>(entity =>
        {
            entity.HasKey(au => new { au.UserId, au.AllergyId }); // composite key

            entity.HasOne(au => au.Allergy)
            .WithMany(a => a.AllergyUser)
            .HasForeignKey(au => au.AllergyId)
            .OnDelete(DeleteBehavior.Cascade); // sletter allergyuser når allergy slettes

            entity.HasIndex(au => au.UserId);

        });


        modelBuilder.Entity<Scan>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.UserId)
                .IsRequired();

            entity.Property(s => s.ScannedAt)
                .IsRequired();

            entity.Property(s => s.Name)
            .HasMaxLength(100);

            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.ScannedAt);

        });


        modelBuilder.Entity<ScanDetectedAllergies>(entity =>
        {
            entity.HasKey(sda => sda.Id);

            entity.HasOne(sda => sda.Scan)
                .WithMany(s => s.DetectedAllergies)
                .HasForeignKey(sda => sda.ScanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sda => sda.Allergy)
                .WithMany(a => a.ScanDetectedAllergies)
                .HasForeignKey(sda => sda.AllergyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sda => sda.CustomAllergy)
                .WithMany()
                .HasForeignKey(sda => sda.CustomAllergyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(sda => new { sda.ScanId, sda.AllergyId })
                .IsUnique();

            entity.HasIndex(sda => new { sda.ScanId, sda.CustomAllergyId })
                .IsUnique();
        });


        modelBuilder.Entity<DetectedIngredientMatch>(entity =>
        {
            entity.HasKey(dim => dim.Id);

            entity.Property(dim => dim.IngredientText)
            .IsRequired().HasMaxLength(200);

            entity.HasOne(dim => dim.ScanDetectedAllergy)
            .WithMany(dim => dim.MatchedIngredients)
            .HasForeignKey(dim => dim.ScanDetectedAllergyId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(dim => new { dim.ScanDetectedAllergyId, dim.IngredientText })
            .IsUnique();

        });

        modelBuilder.Entity<CustomAllergy>(entity =>
        {
            entity.HasKey(ca => ca.Id);

            entity.Property(ca => ca.UserId)
                .IsRequired();

            entity.Property(ca => ca.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(ca => ca.NormalizedName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(ca => new { ca.UserId, ca.NormalizedName })
                .IsUnique();

        });


    }

}
