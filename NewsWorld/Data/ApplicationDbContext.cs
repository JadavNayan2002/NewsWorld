using Microsoft.EntityFrameworkCore;
using NewsWorld.Models;

namespace NewsWorld.Data   // ✅ MUST match this
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
    }
}