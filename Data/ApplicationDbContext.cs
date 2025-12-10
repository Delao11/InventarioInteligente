using Microsoft.EntityFrameworkCore;
using InventarioInteligente.Models;

namespace InventarioInteligente.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
