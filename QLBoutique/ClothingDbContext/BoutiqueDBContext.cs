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
        public DbSet<ChiTietHoaDon> ChiTietHoaDon { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }

        public DbSet<KhuyenMai> KhuyenMai { get; set; }
        public DbSet<LoaiKhuyenMai> LoaiKhuyenMai { get; set; }
        public DbSet<LichSuDiem> LichSuDiem { get; set; }
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToan { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<DiaChiKhachHang> DiaChiKhachHang { get; set; }
        public DbSet<DonViVanChuyen> DonViVanChuyen { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.MaCV);
                entity.Property(e => e.TenCV).IsRequired().HasMaxLength(150);
            });

            modelBuilder.Entity<QuyenHan>(entity =>
            {
                entity.HasKey(e => e.MaQuyen);
                entity.Property(e => e.MoTa).HasMaxLength(200);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNV);
                entity.Property(e => e.HoTen).HasMaxLength(100);
                entity.Property(e => e.DiaChi).HasMaxLength(200);
                entity.Property(e => e.SDT).HasMaxLength(10).IsFixedLength();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.MaQuyen).HasColumnName("MaQuyen");
                entity.Property(e => e.MaCV).HasColumnName("MaCV");

                entity.HasOne(e => e.Quyen)
                      .WithMany()
                      .HasForeignKey(e => e.MaQuyen)
                      .HasPrincipalKey(qh => qh.MaQuyen)  // xác định khóa chính bên QuyenHan
                      .HasConstraintName("FK_NhanVien_QuyenHan");

                entity.HasOne(e => e.ChucVu)
                      .WithMany()
                      .HasForeignKey(e => e.MaCV)
                      .HasPrincipalKey(cv => cv.MaCV);     
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
                      .HasMaxLength(30)
                      .IsRequired();

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
                      .OnDelete(DeleteBehavior.Restrict);
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
                      .HasMaxLength(100)
                      .IsRequired();

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
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.MaNCC)
                      .HasColumnName("MANCC")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TrangThai)
                      .HasColumnName("TRANGTHAI")
                      .HasDefaultValue(1);

                entity.HasOne(e => e.LoaiSanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaLoai)
                      .HasConstraintName("FK_SanPham_LoaiSanPham")
                      .OnDelete(DeleteBehavior.Restrict);

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

                entity.HasOne(e => e.SanPham)
                      .WithMany()
                      .HasForeignKey(e => e.MaSanPham)
                      .HasConstraintName("FK_BIEN_THE_SANPHAM_SANPHAM")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.MaSanPham, e.Size, e.MauSac }).IsUnique();
            });

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

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKH);

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

                entity.HasOne(e => e.LoaiKhachHang)
                    .WithMany()
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

                entity.HasOne(e => e.NhaCungCap)
                      .WithMany()
                      .HasForeignKey(e => e.MaNCC)
                      .HasConstraintName("FK_PhieuNhap_NhaCungCap")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NhanVien)
                      .WithMany()
                      .HasForeignKey(e => e.MaNV)
                      .HasConstraintName("FK_PhieuNhap_NhanVien")
                      .OnDelete(DeleteBehavior.Restrict);
            });

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

                entity.HasOne(e => e.PhieuNhap)
                      .WithMany(p => p.ChiTietPhieuNhaps)
                      .HasForeignKey(e => e.MaPhieuNhap)
                      .HasConstraintName("FK_ChiTietPhieuNhap_PhieuNhap")
                      .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<LoaiKhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaLoaiKM);
                entity.Property(e => e.TenLoaiKM).HasMaxLength(100).IsRequired();
                entity.Property(e => e.GhiChu).HasMaxLength(200);
            });

            modelBuilder.Entity<LichSuDiem>(entity =>
            {
                entity.HasKey(e => e.MaLSD);
                entity.Property(e => e.MaKH)
                    .HasColumnName("MAKH")
                    .HasMaxLength(20)
                    .IsRequired();
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
                entity.HasOne(e => e.KhachHang)
                    .WithMany()
                    .HasForeignKey(e => e.MaKH)
                    .HasConstraintName("FK_LichSuDiem_KhachHang")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhuongThucThanhToan>(entity =>
            {
                entity.HasKey(e => e.MaTT);
                entity.Property(e => e.TenTT)
                    .HasColumnName("TenTT")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<DiaChiKhachHang>(entity =>
            {
                entity.ToTable("DIACHI_KHACHHANG");

                entity.HasKey(e => e.MaDiaChi);

                entity.Property(e => e.MaDiaChi)
                      .HasMaxLength(20);

                entity.Property(e => e.MaKH)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.HoTenNguoiNhan)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.SDTNguoiNhan)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(e => e.DiaChiCuThe)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.TinhTP)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.HuyenQuan)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.XaPhuong)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.GhiChu)
                      .HasMaxLength(200);

                entity.Property(e => e.MacDinh)
                      .HasDefaultValue(false);

                entity.HasOne(e => e.KhachHang)
                      .WithMany(kh => kh.DiaChiKhachHangs)
                      .HasForeignKey(e => e.MaKH)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_DiaChiKhachHang_KhachHang");
            });
            modelBuilder.Entity<DonViVanChuyen>(entity =>
            {
                entity.ToTable("DONVIVANCHUYEN");

                entity.HasKey(e => e.MaDVVC);

                entity.Property(e => e.MaDVVC)
                      .HasColumnName("MADVVC")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.TenDVVC)
                      .HasColumnName("TENDVVC")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.PhiVanChuyen)
                      .HasColumnName("PHIVANCHUYEN")
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(e => e.ThoiGianGiao)
                      .HasColumnName("THOIGIAN_GIAO")
                      .HasMaxLength(50)
                      .IsRequired();
            });
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.ToTable("HOADON");

                entity.HasKey(e => e.MaHoaDon);

                entity.Property(e => e.MaHoaDon)
                      .HasMaxLength(20);

                entity.Property(e => e.MaKH)
                      .HasMaxLength(20);
                entity.Property(e => e.MaDVVC)
                      .HasMaxLength(20);

                entity.Property(e => e.MaNV)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.NgayLap)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.TongTien)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.GiamGia)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ThanhTien)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.MaKM)
                      .HasMaxLength(20);

                entity.Property(e => e.MaTT)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.TrangThai)
                      .HasDefaultValue(1);

                entity.Property(e => e.GhiChu)
                      .HasMaxLength(200);

                entity.Property(e => e.MaDiaChi)
                      .HasMaxLength(20);

                entity.HasOne(e => e.KhachHang)
                      .WithMany()
                      .HasForeignKey(e => e.MaKH)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_HoaDon_KhachHang");

                entity.HasOne(e => e.NhanVien)
                      .WithMany()
                      .HasForeignKey(e => e.MaNV)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_HoaDon_NhanVien");

                entity.HasOne(e => e.KhuyenMai)
                      .WithMany()
                      .HasForeignKey(e => e.MaKM)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_HoaDon_KhuyenMai");

                entity.HasOne(e => e.PhuongThucThanhToan)
                      .WithMany(p => p.HoaDons)
                      .HasForeignKey(e => e.MaTT)
                      .OnDelete(DeleteBehavior.Restrict);
                      

                entity.HasOne(e => e.DiaChiKhachHang)
                      .WithMany(d => d.HoaDons)
                      .HasForeignKey(e => e.MaDiaChi)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_HoaDon_DiaChiKhachHang");
                entity.HasOne(e => e.DonViVanChuyen)
                      .WithMany(d => d.HoaDons)
                      .HasForeignKey(e => e.MaDVVC)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_HoaDon_DonViVanChuyen");
            });

            //modelBuilder.Entity<ChiTietHoaDon>(entity =>
            //{
            //    entity.ToTable("CHITIET_HOADON");
            //    entity.HasKey(e => new { e.MaChiTietHD});

            //    entity.Property(e => e.MaHD)
            //          .HasColumnName("MAHD")
            //          .HasMaxLength(20)
            //          .IsRequired();

            //    entity.Property(e => e.MaBienThe)
            //          .HasColumnName("MABIEN_THE")
            //          .HasMaxLength(20)
            //          .IsRequired();

            //    entity.Property(e => e.SoLuong)
            //          .HasColumnName("SOLUONG")
            //          .HasDefaultValue(1);

            //    entity.Property(e => e.GiaBan)
            //              .HasColumnType("decimal(18,2)");

            //    entity.Property(e => e.GiaGiam)
            //              .HasColumnType("decimal(18,2)");

            //    entity.Property(e => e.ThanhTien)
            //              .HasColumnType("decimal(18,2)");
            //    // Khóa ngoại đến HoaDon
            //    entity.HasOne(e => e.HoaDon)
            //          .WithMany()
            //          .HasForeignKey(e => e.MaHD)
            //          .HasConstraintName("FK_ChiTietHoaDon_HoaDon")
            //          .OnDelete(DeleteBehavior.Cascade);

            //    // Khóa ngoại đến BIEN_THE_SANPHAM
            //    entity.HasOne(e => e.ChiTietSanPham)
            //          .WithMany()
            //          .HasForeignKey(e => e.MaBienThe)
            //          .HasConstraintName("FK_ChiTietHoaDon_ChiTietSanPham")
            //          .OnDelete(DeleteBehavior.Restrict);
            //});
        }
    }
}
