using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // table based on prompt entity
    public DbSet<Prompt> Prompts => Set<Prompt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enum mapped to string for better readability 
        modelBuilder.Entity<Prompt>()
            .Property(p => p.Status)
            .HasConversion<string>();
    }
}