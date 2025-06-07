using QLBoutique.Model;
using Microsoft.EntityFrameworkCore;
using LabManagement.Model;

namespace QLBoutique.ClothingDbContext
{
    public class BoutiqueDBContext : DbContext
    {
        public BoutiqueDBContext(DbContextOptions<BoutiqueDBContext> options) : base(options)
        { }

        public DbSet<ChucVu> ChucVu { get; set; }
        public DbSet<QuyenHan> QuyenHan { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPham { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<ChiTietSanPham> ChiTietSanPham { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<LoaiKhachHang> LoaiKhachHang { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHang { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }

        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<LoaiKhuyenMai> LoaiKhuyenMai { get; set; }
        public DbSet<LichSuDiem> LichSuDiems { get; set; }
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToan { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.MaCV);
                entity.Property(e => e.TenCV).IsRequired().HasMaxLength(150);
            });
            // Cấu hình bảng QuyenHan
            modelBuilder.Entity<QuyenHan>(entity =>
            {
                entity.HasKey(e => e.MaQuyen);
                entity.Property(e => e.MoTa).HasMaxLength(200);
            });

            // Cấu hình bảng NhanVien
            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNV);
            
                entity.Property(e => e.HoTen).HasMaxLength(100);
                entity.Property(e => e.DiaChi).HasMaxLength(200);
                entity.Property(e => e.SDT).HasMaxLength(10).IsFixedLength();
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            
                // Thiết lập quan hệ với bảng QuyenHan
                entity.HasOne(e => e.QuyenHan)
                      .WithMany()
                      .HasForeignKey(e => e.MaQuyen)
                      .HasConstraintName("FK_NhanVien_QuyenHan");
            
                // Thiết lập quan hệ với bảng ChucVu
                entity.HasOne(e => e.ChucVu)
                      .WithMany()
                      .HasForeignKey(e => e.MaCV)
                      .HasConstraintName("FK_NhanVien_ChucVu");
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

            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.ToTable("GIOHANG");

                entity.HasKey(e => e.MaGioHang);

                entity.Property(e => e.MaGioHang)
                      .HasColumnName("MAGIOHANG")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaKH)
                      .HasColumnName("MAKH")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.NgayTao)
                      .HasColumnName("NGAYTAO");

                entity.Property(e => e.NgayCapNhat)
                      .HasColumnName("NGAYCAPNHAT");

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);

                // Thiết lập quan hệ với bảng KhachHang
                entity.HasOne(e => e.KhachHang)
                      .WithMany() // hoặc WithMany(k => k.DanhSachGioHang) nếu bạn có collection
                      .HasForeignKey(e => e.MaKH)
                      .HasConstraintName("FK_GioHang_KhachHang")
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.ToTable("CHITIET_GIOHANG");

                entity.HasKey(e => new { e.MaGioHang, e.MaBienThe });

                entity.Property(e => e.MaGioHang)
                      .HasColumnName("MAGIOHANG")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaBienThe)
                      .HasColumnName("MABIEN_THE")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.SoLuong)
                      .HasColumnName("SOLUONG")
                      .HasDefaultValue(1);

                // Khóa ngoại đến GIOHANG
                entity.HasOne(e => e.GioHang)
                      .WithMany()
                      .HasForeignKey(e => e.MaGioHang)
                      .HasConstraintName("FK_ChiTietGioHang_GioHang")
                      .OnDelete(DeleteBehavior.Cascade);

                // Khóa ngoại đến BIEN_THE_SANPHAM
                entity.HasOne(e => e.ChiTietSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaBienThe)
                      .HasConstraintName("FK_ChiTietGioHang_ChiTietSanPham")
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // Cấu hình bảng PHIEUNHAP
            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.ToTable("PHIEUNHAP");

                entity.HasKey(e => e.MaPhieuNhap);

                entity.Property(e => e.MaPhieuNhap)
                      .HasColumnName("MAPHIEUNHAP")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaNCC)
                      .HasColumnName("MANCC")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaNV)
                      .HasColumnName("MANV")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.NgayNhap)
                      .HasColumnName("NGAYNHAP");

                entity.Property(e => e.TongTien)
                      .HasColumnName("TONGTIEN")
                      .HasColumnType("decimal(18,2)")
                      .HasDefaultValue(0);

                entity.Property(e => e.GhiChu)
                      .HasColumnName("GHICHU")
                      .HasMaxLength(200);

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);

                // Quan hệ với NhaCungCap
                entity.HasOne(e => e.NhaCungCap)
                      .WithMany()
                      .HasForeignKey(e => e.MaNCC)
                      .HasConstraintName("FK_PhieuNhap_NhaCungCap")
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với NhanVien
                entity.HasOne(e => e.NhanVien)
                      .WithMany()
                      .HasForeignKey(e => e.MaNV)
                      .HasConstraintName("FK_PhieuNhap_NhanVien")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình bảng CHITIET_PHIEUNHAP
            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.ToTable("CHITIET_PHIEUNHAP");

                entity.HasKey(e => new { e.MaPhieuNhap, e.MaBienThe });

                entity.Property(e => e.MaPhieuNhap)
                      .HasColumnName("MAPHIEUNHAP")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaBienThe)
                      .HasColumnName("MABIEN_THE")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.SoLuong)
                      .HasColumnName("SOLUONG")
                      .IsRequired();

                entity.Property(e => e.Gia_Von)
                      .HasColumnName("GIA_VON")
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                // Khóa ngoại đến PhieuNhap
                entity.HasOne(e => e.PhieuNhap)
                      .WithMany(p => p.ChiTietPhieuNhaps)
                      .HasForeignKey(e => e.MaPhieuNhap)
                      .HasConstraintName("FK_ChiTietPhieuNhap_PhieuNhap")
                      .OnDelete(DeleteBehavior.Cascade);

                // Khóa ngoại đến ChiTietSanPham (Biến thể sản phẩm)
                entity.HasOne(e => e.BienTheSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaBienThe)
                      .HasConstraintName("FK_ChiTietPhieuNhap_ChiTietSanPham")
                      .OnDelete(DeleteBehavior.Restrict);
            });
                // Khuyến mãi
                modelBuilder.Entity<KhuyenMai>(entity =>
                {
                    entity.HasKey(e => e.MaKM);
                    entity.Property(e => e.TenKM).HasMaxLength(100);
                    entity.Property(e => e.MoTa).HasMaxLength(200);
                    entity.Property(e => e.MaLoaiKM).HasMaxLength(20);
                    entity.Property(e => e.PhanTramGiam);
                    //entity.Property(e => e.GiamToiDa);
                    //entity.Property(e => e.GiamTien); 
                    //entity.Property(e => e.DieuKien);
                    entity.Property(e => e.NgayBatDau);
                    entity.Property(e => e.NgayKetThuc);
                    entity.Property(e => e.TrangThai);
                    entity.Property(e => e.SoLuongApDung);
                    entity.Property(e => e.SoLuongDaApDung);
                    // Thiết lập mối quan hệ với KhachHang
                    entity.HasOne(e => e.LoaiKhuyenMai)
                        .WithMany()
                        .HasForeignKey(e => e.MaLoaiKM)
                        .HasConstraintName("FK_KhuyenMai_LoaiKhuyenMai");

                });


                //Loại khuyến mãi
                modelBuilder.Entity<LoaiKhuyenMai>(entity =>
                {
                    entity.HasKey(e => e.MaLoaiKM);
                    entity.Property(e => e.TenLoaiKM).HasMaxLength(100);
                    entity.Property(e => e.GhiChu).HasMaxLength(200);
                });

                // Lịch sử điểm
                modelBuilder.Entity<LichSuDiem>(entity =>
                {
                    entity.HasKey(e => e.MaLSD); // Khóa chính

                    entity.Property(e => e.MaKH)
                        .HasColumnName("MAKH")
                        .HasMaxLength(20);

                    entity.Property(e => e.Ngay)
                        .HasColumnName("NGAY")
                        .IsRequired();

                    entity.Property(e => e.Diem)
                        .HasColumnName("DIEM")
                        .IsRequired();

                    entity.Property(e => e.Loai)
                        .HasColumnName("LOAI")
                        .HasMaxLength(20);

                    entity.Property(e => e.GhiChu)
                        .HasColumnName("GHICHU")
                        .HasMaxLength(200);

                    // Thiết lập mối quan hệ với KhachHang
                    entity.HasOne(e => e.KhachHang)
                        .WithMany()
                        .HasForeignKey(e => e.MaKH)
                        .HasConstraintName("FK_LichSuDiem_KhachHang");
                });

                //Giỏ Hàng
                modelBuilder.Entity<GioHang>(entity =>
                {
                    entity.HasKey(e => e.MaGioHang); // Khóa chính

                    entity.Property(e => e.MaKH)
                        .HasColumnName("MAKH")
                        .HasMaxLength(20);

                    entity.Property(e => e.NgayTao)
                        .HasColumnName("NGAYTAO")
                        .IsRequired();

                    entity.Property(e => e.NgayCapNhat)
                        .HasColumnName("NGAYCAPNHAT")
                        .IsRequired();

                    entity.Property(e => e.TrangThai)
                        .HasColumnName("TRANGTHAI")
                        .IsRequired();

                    // Thiết lập mối quan hệ với KhachHang
                    entity.HasOne(e => e.KhachHang)
                        .WithMany()
                        .HasForeignKey(e => e.MaKH)
                        .HasConstraintName("FK_GioHang_KhachHang");
                });

                //Phương thức thanh toán
                modelBuilder.Entity<PhuongThucThanhToan>(entity =>
                {
                    entity.HasKey(e => e.MaTT); // Khóa chính

                    entity.Property(e => e.TenTT)
                        .HasColumnName("TenTT")
                        .HasMaxLength(50);
                });

                //Hóa đơn
                modelBuilder.Entity<HoaDon>(entity =>
                {
                    entity.HasKey(e => e.MaHoaDon); // Khóa chính

                    entity.Property(e => e.MaKH)
                        .HasColumnName("MAKH")
                        .HasMaxLength(20);
                    entity.Property(e => e.MaNV)
                        .HasColumnName("MANV")
                        .HasMaxLength(20);
                    entity.Property(e => e.NgayLap)
                        .HasColumnName("NGAYLAP")
                        .IsRequired();
                    entity.Property(e => e.TongTien)
                          .HasColumnType("decimal(18,2)");
                    entity.Property(e => e.GiamGia)
                          .HasColumnType("decimal(18,2)");
                    entity.Property(e => e.ThanhTien)
                          .HasColumnType("decimal(18,2)");
                    entity.Property(e => e.MaKM)
                        .HasColumnName("MAKM")
                        .HasMaxLength(20);
                    entity.Property(e => e.MaTT)
                        .HasColumnName("MATT")
                        .HasMaxLength(20);
                    entity.Property(e => e.TrangThai)
                        .HasColumnName("TRANGTHAI")
                        .IsRequired();
                    entity.Property(e => e.GhiChu)
                        .HasColumnName("GHICHU")
                        .HasMaxLength(200);

                    // Thiết lập mối quan hệ với KhachHang
                    entity.HasOne(e => e.KhachHang)
                        .WithMany()
                        .HasForeignKey(e => e.MaKH)
                        .HasConstraintName("FK_HoaDon_KhachHang");
                    // Thiết lập mối quan hệ với NhanVien
                    entity.HasOne(e => e.Nhanvien)
                        .WithMany()
                        .HasForeignKey(e => e.MaNV)
                        .HasConstraintName("FK_HoaDon_NhanVien");

                    // Thiết lập mối quan hệ với KhuyenMai
                    entity.HasOne(e => e.KhuyenMai)
                        .WithMany()
                        .HasForeignKey(e => e.MaKM)
                        .HasConstraintName("FK_HoaDon_KhuyenMai");
                    // Thiết lập mối quan hệ với PhuongThucThanhToan
                    entity.HasOne(e => e.PhuongThucThanhToan)
                        .WithMany()
                        .HasForeignKey(e => e.MaTT)
                        .HasConstraintName("FK_HoaDon_PhuongThucThanhToan");
                });

        }
    }
}
