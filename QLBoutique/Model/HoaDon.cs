using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("HOADON")]
    public class HoaDon
    {
        [Key]
        [Column("MAHD")]
        [StringLength(20)]
        public string MaHoaDon { get; set; }

        [Column("MAKH")]
        [StringLength(20)]
        public string? MaKH { get; set; }

        [Column("MANV")]
        [StringLength(20)]
        public string MaNV { get; set; }

        [Column("NGAYLAP")]
        public DateTime NgayLap { get; set; }

        [Column("TONGTIEN")]
        public decimal TongTien{ get; set; }

        [Column("GiamGia")]
        public decimal GiamGia { get; set; }
        [Column("THANHTIEN")]
        public decimal ThanhTien { get; set; }
       
        [Column("MAKM")]
        [StringLength(20)]
        public string? MaKM { get; set; }

        [Column("MATT")]
        [StringLength(20)]
        public string MaTT { get; set; }

        [Column("TRANGTHAI")]
        public int? TrangThai { get; set; }

        [Column("GHICHU")]
        [StringLength(200)]
        public string? GhiChu { get; set; }


        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien? Nhanvien { get; set; }

        [ForeignKey("MaKM")]
        public virtual KhuyenMai? KhuyenMai { get; set; }

        [ForeignKey("MaTT")]
        public virtual PhuongThucThanhToan? PhuongThucThanhToan { get; set; }
    }
}
