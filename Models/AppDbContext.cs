using Microsoft.EntityFrameworkCore;

namespace ConsultorPrecio.Models {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        public DbSet<Producto> Productos { get; set; }

    }
}
