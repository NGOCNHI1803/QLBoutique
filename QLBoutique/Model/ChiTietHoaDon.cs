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
        public string MaChiTietHD { get; set; }

        [Column("MAHD")]
        public string? MaHD { get; set; }

        [Column("MABIEN_THE")]
        public string? MaBienThe { get; set; }

        [Column("SOLUONG")]
        public int SoLuong { get; set; } = 1;

        [Column("GIA_BAN")]
        public decimal? GiaBan { get; set; }

        [Column("GIAMGIA")]
        public decimal? GiaGiam { get; set; }

        [Column("THANHTIEN")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaHD")]
        [JsonIgnore]
        public HoaDon? HoaDon { get; set; }

        [ForeignKey("MaBienThe")]
        [JsonIgnore]
        public ChiTietSanPham? ChiTietSanPham { get; set; }
    }
}
