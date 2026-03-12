using AuthSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthSystemAPI.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

    }

}
