using Microsoft.EntityFrameworkCore;
using ProjectDashboard.Models;

namespace ProjectDashboard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
    }
}
