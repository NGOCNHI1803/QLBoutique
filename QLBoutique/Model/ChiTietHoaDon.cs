using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    [Table("CHITIET_HOADON")]
    public class ChiTietHoaDon
    {
        [Key]
        [Column("MACHITIET_HD")]
        [StringLength(20)]
        public string MaChiTiet_HD { get; set; }

        [Column("MAHD")]
        [StringLength(20)]
        public string MaHD { get; set; }

        [Column("MABIEN_THE")]
        [StringLength(20)]
        public string MaBienThe { get; set; }

        [Column("SOLUONG")]
        public int SoLuong { get; set; }

        [Column("GIA_BAN")]
        public decimal GiaBan { get; set; }

        [Column("GIAMGIA")]
        public decimal GiamGia { get; set; }

        [Column("THANHTIEN")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaHD")]
        [JsonIgnore] // Ignore vòng tham chiếu về HoaDon khi serialize
        public HoaDon HoaDon { get; set; }

        [ForeignKey("MaBienThe")]
        public ChiTietSanPham BienTheSanPham { get; set; }
    }
}
