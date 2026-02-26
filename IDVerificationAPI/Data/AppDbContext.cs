using IDVerificationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IDVerificationAPI.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This makes the NationalId column unique across the whole table
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NationalId)
                .IsUnique();
        }
    }

}
