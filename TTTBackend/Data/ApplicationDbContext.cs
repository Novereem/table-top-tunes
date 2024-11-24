using Microsoft.EntityFrameworkCore;
using Shared.Models.Sounds;
using Shared.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Scene> Scenes { get; set; }
    public DbSet<SoundPreset> SoundPresets { get; set; }
    public DbSet<PresetSound> PresetSounds { get; set; }
    public DbSet<AudioFile> AudioFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure GUIDs as CHAR(36) for MySQL
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

        // User -> Scenes
        modelBuilder.Entity<Scene>()
            .HasOne(s => s.User)
            .WithMany(u => u.Scenes)
            .HasForeignKey(s => s.UserId)
            .IsRequired();

        // Scene -> AudioFiles
        modelBuilder.Entity<AudioFile>()
            .HasOne(af => af.Scene)
            .WithMany(s => s.AudioFiles)
            .HasForeignKey(af => af.SceneId)
            .IsRequired(false);

        // User -> AudioFiles
        modelBuilder.Entity<AudioFile>()
            .HasOne(af => af.User)
            .WithMany(u => u.AudioFiles)
            .HasForeignKey(af => af.UserId)
            .IsRequired();

        // Scene -> SoundPresets
        modelBuilder.Entity<SoundPreset>()
            .HasOne(sp => sp.Scene)
            .WithMany(s => s.SoundPresets)
            .HasForeignKey(sp => sp.SceneId)
            .IsRequired();

        // SoundPreset -> PresetSounds
        modelBuilder.Entity<PresetSound>()
            .HasOne(ps => ps.SoundPreset)
            .WithMany(sp => sp.PresetSounds)
            .HasForeignKey(ps => ps.SoundPresetId)
            .IsRequired();

        // PresetSound -> AudioFile
        modelBuilder.Entity<PresetSound>()
            .HasOne(ps => ps.Sound)
            .WithOne()
            .HasForeignKey<PresetSound>(ps => ps.SoundId)
            .IsRequired(false);

        // Enum Configurations
        modelBuilder.Entity<AudioFile>()
            .Property(af => af.Type)
            .HasConversion<string>();
    }
}