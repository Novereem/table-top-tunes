using Microsoft.EntityFrameworkCore;
using Shared.Models;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users { get; set; }

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
					property.SetColumnType("CHAR(36)"); // Apply globally for all GUIDs
				}
			}
		}
	}
}