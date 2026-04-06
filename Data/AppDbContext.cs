using backend_marsh_project.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_marsh_project.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
    }
}
