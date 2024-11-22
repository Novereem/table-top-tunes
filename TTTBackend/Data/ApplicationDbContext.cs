using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Models.Sounds.Presets;
using Shared.Models.Sounds;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Scene> Scenes { get; set; }
    public DbSet<MusicTrack> MusicTracks { get; set; }
    public DbSet<AmbientSound> AmbientSounds { get; set; }
    public DbSet<SoundEffect> SoundEffects { get; set; }
    public DbSet<SoundPreset> SoundPresets { get; set; }
    public DbSet<PresetSound> PresetSounds { get; set; }
    public DbSet<AudioFile> AudioFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically configure all Guid properties in the model to use CHAR(36) in MySQL
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Guid))
                {
                    property.SetColumnType("CHAR(36)");
                }
            }
        }

        // User -> Scenes (One-to-Many)
        modelBuilder.Entity<Scene>()
            .HasOne(s => s.User)
            .WithMany(u => u.Scenes)
            .IsRequired();

        // Scene -> Audio Components (One-to-Many)
        modelBuilder.Entity<MusicTrack>()
            .HasOne(mt => mt.Scene)
            .WithMany(s => s.MusicTracks)
            .IsRequired();

        modelBuilder.Entity<AmbientSound>()
            .HasOne(ams => ams.Scene)
            .WithMany(s => s.AmbientSounds)
            .IsRequired();

        modelBuilder.Entity<SoundEffect>()
            .HasOne(se => se.Scene)
            .WithMany(s => s.SoundEffects)
            .IsRequired();

        modelBuilder.Entity<SoundPreset>()
            .HasOne(sp => sp.Scene)
            .WithMany(s => s.SoundPresets)
            .IsRequired();

        // SoundPreset -> PresetSound (One-to-Many)
        modelBuilder.Entity<PresetSound>()
            .HasOne(ps => ps.SoundPreset)
            .WithMany(sp => sp.PresetSounds)
            .HasForeignKey(ps => ps.SoundPresetId) // Explicit foreign key
            .IsRequired();

        // PresetSound -> AudioFile (One-to-One)
        modelBuilder.Entity<PresetSound>()
            .HasOne(ps => ps.Sound) // PresetSound.Sound navigates to AudioFile
            .WithOne()              // AudioFile does not reference PresetSound
            .HasForeignKey<PresetSound>(ps => ps.Id) // Foreign key on PresetSound
            .IsRequired(false); // Optional, set to true if mandatory

        // Configure enum to be stored as strings instead of integers
        modelBuilder.Entity<PresetSound>()
            .Property(ps => ps.SoundType)
            .HasConversion<string>();
    }
}
