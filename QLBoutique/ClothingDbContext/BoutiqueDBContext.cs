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
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<LoaiKhachHang> LoaiKhachHang { get; set; }


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
            // Thêm vào cuối OnModelCreating
            modelBuilder.Entity<LoaiKhachHang>(entity =>
            {
                entity.ToTable("LOAI_KHACHHANG");

                entity.HasKey(e => e.MaLoaiKH);

                entity.Property(e => e.MaLoaiKH)
                      .HasMaxLength(20);

                entity.Property(e => e.TenLoaiKH)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.DieuKienTongChi)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.PhanTramGiam)
                      .HasDefaultValue(0)
                      .IsRequired();

                entity.Property(e => e.MoTa)
                      .HasMaxLength(200);
            });

            // Cấu hình cho bảng KhachHang
            modelBuilder.Entity<KhachHang>(entity =>
            {
                // Khóa chính
                entity.HasKey(e => e.MaKH);

                // Các property theo đúng cấu trúc class
                entity.Property(e => e.MaKH)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.MatKhau)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.TenKH)
                    .HasMaxLength(100);

                entity.Property(e => e.DiaChi)
                    .HasMaxLength(100);

                entity.Property(e => e.SoDienThoai)
                    .HasMaxLength(10);

                entity.Property(e => e.NgaySinh);

                entity.Property(e => e.GhiChu)
                    .HasMaxLength(100)
                    .HasDefaultValue("Khách hàng mới");

                entity.Property(e => e.MaLoaiKH)
                    .HasMaxLength(20)
                    .HasDefaultValue("KHT");

                entity.Property(e => e.NgayDangKy)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TrangThai)
                    .HasMaxLength(20)
                    .HasDefaultValue("Hoạt động");

                // Quan hệ khóa ngoại (nếu muốn cấu hình)
                entity.HasOne(e => e.LoaiKhachHang)
                    .WithMany()  // hoặc .WithMany(l => l.DsKhachHang) nếu có collection trong LoaiKhachHang
                    .HasForeignKey(e => e.MaLoaiKH)
                    .OnDelete(DeleteBehavior.Restrict);

            });

        }
    }
}
