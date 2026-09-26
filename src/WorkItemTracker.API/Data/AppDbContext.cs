using Microsoft.EntityFrameworkCore;
using WorkItemTracker.API.Models;

namespace WorkItemTracker.API.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<WorkItem> WorkItems => Set<WorkItem>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var entity = modelBuilder.Entity<WorkItem>();

			entity.HasKey(x => x.Id);

			entity.Property(x => x.Title)
				.IsRequired()
				.HasMaxLength(120);

			entity.Property(x => x.Description);

			entity.Property(x => x.Status)
				.HasConversion<string>()
				.IsRequired();

			entity.Property(x => x.CreatedAt)
				.IsRequired();

			entity.HasIndex(x => x.Title);
			entity.HasIndex(x => x.Status);
		}
	}
}
