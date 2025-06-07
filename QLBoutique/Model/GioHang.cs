using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("GIOHANG")]
    public class GioHang
    {
<<<<<<< HEAD
        public string? MaGioHang { get; set; } // Mã giỏ hàng (MAGIOHANG)
=======
        [Key]
        [Column("MAGIOHANG")]
        [StringLength(20)]
        public string MaGioHang { get; set; }

        [Column("MAKH")]
        [StringLength(100)]
        public string? MaKH { get; set; }

        [Column("NGAYTAO")]
        public DateTime NgayTao { get; set; }

        [Column("NGAYCAPNHAT")]
        public DateTime NgayCapNhat { get; set; }

        [Column("TRANGTHAI")]
        public int? TrangThai{ get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }


>>>>>>> dbd1ab9 (Update backend)

        public string? MaKhachHang { get; set; } // Mã khách hàng (MAKH)

        public DateTime? NgayTao { get; set; }  // Ngày tạo (NGAYTAO)

        public DateTime? NgayCapNhat { get; set; } 
        public int TrangThai { get; set; } = 1; // 1: còn hiệu lực, 0: đã đặt hàng hoặc hủy

        // Navigation property tới bảng KhachHang
        [ForeignKey("MaKhachHang")]
        [JsonIgnore]
        public KhachHang? KhachHang { get; set; }
    }
}
