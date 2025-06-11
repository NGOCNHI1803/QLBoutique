using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("KHACHHANG")]
    public class KhachHang
    {
        [Key]
        [Column("MAKH")]
        [StringLength(20)]
        public string? MaKH { get; set; }

        [Column("EMAIL")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Column("USERNAME")]
        [StringLength(50)]
        public string? TaiKhoan { get; set; }

        [Column("PASSWORD")]
        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; }

        [Column("TENKH")]
        [StringLength(100)]
        public string? TenKH { get; set; }

        [Column("DIACHI")]
        [StringLength(100)]
        public string? DiaChi { get; set; }

        [Column("SDT")]
        [StringLength(10)]
        public string? SoDienThoai { get; set; }

        [Column("NGAYSINH")]
        public DateTime? NgaySinh { get; set; }

        [Column("GHICHU")]
        [StringLength(100)]
        public string? GhiChu { get; set; } = "Khách hàng mới";

        [Column("MALOAIKH")]
        [StringLength(20)]
        public string? MaLoaiKH { get; set; } = "KHT";

        [Column("NGAYDANGKY")]
        public DateTime? NgayDangKy { get; set; } = DateTime.Now;

        [Column("TRANGTHAI")]
        [StringLength(20)]
        public string? TrangThai { get; set; } = "Hoạt động";

        // Navigation property (optional if you want to link LoaiKhachHang)
        [ForeignKey("MaLoaiKH")]
        public virtual LoaiKhachHang? LoaiKhachHang { get; set; }


        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }

        public ICollection<DiaChiKhachHang> DiaChiKhachHangs { get; set; }


    }
}
