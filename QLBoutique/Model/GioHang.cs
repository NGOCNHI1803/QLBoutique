using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("GIOHANG")]
    public class GioHang
    {
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
        // Navigation property tới bảng KhachHang
        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }
    }
}
