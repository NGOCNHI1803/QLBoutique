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
                entity.ToTable("LOAI_SANPHAM");

                entity.HasKey(e => e.MaLoai);

                entity.Property(e => e.MaLoai)
                      .HasColumnName("MALOAI")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TenLoai)
                      .HasColumnName("TENLOAI")
                      .HasMaxLength(30);

                entity.Property(e => e.XuatSu)
                      .HasColumnName("XUATSU")
                      .HasMaxLength(100);

                entity.Property(e => e.ParentId)
                      .HasColumnName("PARENT_ID")
                      .HasMaxLength(20);

                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .HasConstraintName("FK_LoaiSanPham_Parent")
                      .OnDelete(DeleteBehavior.Restrict); // để tránh xóa cascade nếu cần
            });
            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.ToTable("NHACUNGCAP");

                entity.HasKey(e => e.MaNCC);

                entity.Property(e => e.MaNCC)
                      .HasColumnName("MANCC")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TenNCC)
                      .HasColumnName("TENNCC")
                      .HasMaxLength(100);

                entity.Property(e => e.DiaChi)
                      .HasColumnName("DIACHI")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.SDT)
                      .HasColumnName("SDT")
                      .HasMaxLength(15);

                entity.Property(e => e.Email)
                      .HasColumnName("EMAIL")
                      .HasMaxLength(100);

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);
            });
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.ToTable("SANPHAM");

                entity.HasKey(e => e.MaSanPham);

                entity.Property(e => e.MaSanPham)
                      .HasColumnName("MASP")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TenSanPham)
                      .HasColumnName("TENSP")
                      .HasMaxLength(100);

                entity.Property(e => e.MoTa)
                      .HasColumnName("MOTA");

                entity.Property(e => e.HinhAnh)
                      .HasColumnName("HINHANH");

                entity.Property(e => e.MaLoai)
                      .HasColumnName("MALOAI")
                      .HasMaxLength(20);

                entity.Property(e => e.MaNCC)
                      .HasColumnName("MANCC")
                      .HasMaxLength(20);

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);

                // Quan hệ với LoaiSanPham (không có collection navigation)
                entity.HasOne(e => e.LoaiSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaLoai)
                      .HasConstraintName("FK_SanPham_LoaiSanPham")
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với NhaCungCap (không có collection navigation)
                entity.HasOne(e => e.NhaCungCap)
                      .WithMany()
                      .HasForeignKey(e => e.MaNCC)
                      .HasConstraintName("FK_SanPham_NhaCungCap")
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ChiTietSanPham>(entity =>
            {
                entity.ToTable("BIEN_THE_SANPHAM");

                entity.HasKey(e => e.MaBienThe);

                entity.Property(e => e.MaBienThe)
                      .HasColumnName("MABIEN_THE")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaSanPham)
                      .HasColumnName("MASP")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Size)
                      .HasColumnName("SIZE")
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(e => e.MauSac)
                      .HasColumnName("MAUSAC")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.HinhAnh)
                      .HasColumnName("HINHANH")
                      .HasColumnType("TEXT");

                entity.Property(e => e.Barcode)
                      .HasColumnName("BARCODE")
                      .HasMaxLength(30);

                entity.Property(e => e.GiaVon)
                      .HasColumnName("GIA_VON")
                      .HasColumnType("decimal(15,2)");

                entity.Property(e => e.GiaBan)
                      .HasColumnName("GIA_BAN")
                      .HasColumnType("decimal(15,2)");

                entity.Property(e => e.TonKho)
                      .HasColumnName("TON_KHO")
                      .HasDefaultValue(0);

                entity.Property(e => e.TrongLuong)
                      .HasColumnName("TRONGLUONG");

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);

                // Khóa ngoại đến SanPham
                entity.HasOne(e => e.SanPham)
                      .WithMany()   // Nếu SanPham có collection ChiTietSanPham thì thay thành .WithMany(x => x.ChiTietSanPham)
                      .HasForeignKey(e => e.MaSanPham)
                      .HasConstraintName("FK_BIEN_THE_SANPHAM_SANPHAM")
                      .OnDelete(DeleteBehavior.Restrict);

                // Ràng buộc unique trên tổ hợp MASP, SIZE, MAUSAC
                entity.HasIndex(e => new { e.MaSanPham, e.Size, e.MauSac }).IsUnique();
            });
        }
    }
}
