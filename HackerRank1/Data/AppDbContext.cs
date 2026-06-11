using LibraryService.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Fraud> Frauds { get; set; }
    }
}
