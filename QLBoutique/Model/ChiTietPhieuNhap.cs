using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    public class ChiTietPhieuNhap
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        public string? MaPhieuNhap { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(20)]
        public string? MaBienThe { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia_Von { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuNhap")]
        public virtual PhieuNhap? PhieuNhap { get; set; }

        [ForeignKey("MaBienThe")]
        public virtual ChiTietSanPham? BienTheSanPham { get; set; }
    }
}
