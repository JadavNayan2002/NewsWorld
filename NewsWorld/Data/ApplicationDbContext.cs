using Microsoft.EntityFrameworkCore;
using NewsWorld.Models;

namespace NewsWorld.Data   
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}