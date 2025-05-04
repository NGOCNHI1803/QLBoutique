using QLBoutique.Model;
using Microsoft.EntityFrameworkCore;

namespace QLBoutique.ClothingDbContext
{
    public class BoutiqueDBContext : DbContext
    {
        public BoutiqueDBContext(DbContextOptions<BoutiqueDBContext> options) : base(options) 
        {}

        public DbSet<ChucVu> ChucVu { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.MaChucVu);
                entity.Property(e => e.TenChucVu).IsRequired().HasMaxLength(150);
            });
        }
    }
}
