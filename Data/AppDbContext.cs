using Microsoft.EntityFrameworkCore;
using ProfileApi.Models.Entities;

namespace ProfileApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Profile>()
                .Property(p => p.Id)
                .HasDefaultValueSql("(lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))), 2) || '-a' || substr(lower(hex(randomblob(2))), 2) || '-' || lower(hex(randomblob(6))))");

            // Ensure DateTime is UTC
            modelBuilder.Entity<Profile>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
