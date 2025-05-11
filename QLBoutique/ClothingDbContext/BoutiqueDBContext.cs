using QLBoutique.Model;
using Microsoft.EntityFrameworkCore;
using LabManagement.Model;

namespace QLBoutique.ClothingDbContext
{
    public class BoutiqueDBContext : DbContext
    {
        public BoutiqueDBContext(DbContextOptions<BoutiqueDBContext> options) : base(options) 
        {}

        public DbSet<ChucVu> ChucVu { get; set; }

        public DbSet<LoaiSanPham> LoaiSanPham { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<ChiTietSanPham> ChiTietSanPham { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.MaChucVu);
                entity.Property(e => e.TenChucVu).IsRequired().HasMaxLength(150);
            });

            modelBuilder.Entity<LoaiSanPham>(entity =>
            {

                entity.HasKey(e => e.MaLoaiSP);
                entity.Property(e => e.TenLoaiSP).IsRequired().HasMaxLength(150);
            });
            // Cấu hình cho bảng NhaCungCap
            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.MaNCC);  // Đặt khóa chính
                entity.Property(e => e.TenNCC).IsRequired().HasMaxLength(150); // Thuộc tính TenNCC bắt buộc và dài tối đa 150 ký tự
                entity.Property(e => e.ThongTinLienHe).IsRequired().HasMaxLength(150); // Thuộc tính ThongTinLienHe bắt buộc và dài tối đa 150 ký tự
            });
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSanPham);

                entity.Property(e => e.TenSanPham)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.GiaNhap)
                      .IsRequired();

                entity.Property(e => e.GiaBan)
                      .IsRequired();

                entity.Property(e => e.SoLuongTon)
                      .IsRequired();

                entity.Property(e => e.isDeleted)
                      .HasDefaultValue(false).IsRequired();

                // Quan hệ với LoaiSanPham
                entity.HasOne(e => e.LoaiSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaLoai)
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với NhaCungCap
                entity.HasOne(e => e.NhaCungCap)
                      .WithMany()
                      .HasForeignKey(e => e.MaNCC)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChiTietSanPham>(entity =>
            {
                entity.HasKey(e => e.MaChiTiet);

                entity.Property(e => e.MaSKU)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.MoTa)
                      .IsRequired();

                entity.Property(e => e.MauSac)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.KichCo)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(e => e.ChatLieu)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.DacTinh)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Form)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Gia)
                      .IsRequired();

                entity.Property(e => e.SoLuong)
                      .IsRequired();

                entity.Property(e => e.isDeleted)
                      .HasDefaultValue(false).IsRequired();

                // Quan hệ với SanPham
                entity.HasOne(e => e.SanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaSanPham)
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với LoaiSanPham
                entity.HasOne(e => e.LoaiSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaLoaiSP)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
