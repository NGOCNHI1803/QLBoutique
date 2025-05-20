using LabManagement.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class ChiTietSanPham
    {
        [Key]
        [StringLength(20)]
        public string? MaBienThe { get; set; } // MABIEN_THE - khóa chính

        [Required]
        [StringLength(20)]
        public string? MaSanPham { get; set; } // MASP - khóa ngoại

        [Required]
        [StringLength(10)]
        public string? Size { get; set; } // SIZE

        [Required]
        [StringLength(20)]
        public string? MauSac { get; set; } // MAUSAC

        [Column(TypeName = "TEXT")]
        public string? HinhAnh { get; set; } // HINHANH

        [StringLength(30)]
        public string? Barcode { get; set; } // BARCODE

        [Column(TypeName = "decimal(15,2)")]
        public decimal? GiaVon { get; set; } // GIA_VON

        [Column(TypeName = "decimal(15,2)")]
        public decimal? GiaBan { get; set; } // GIA_BAN

        public int TonKho { get; set; } = 0; // TON_KHO

        public float? TrongLuong { get; set; } // TRONGLUONG

        public int TrangThai { get; set; } = 1; // TRANGTHAI


        [NotMapped]
        public string? HinhAnhUrl => string.IsNullOrEmpty(HinhAnh)
            ? null
            : $"https://localhost:7265/images/{HinhAnh}";

        // Navigation property liên kết bảng SanPham
        [ForeignKey("MaSanPham")]
        [JsonIgnore]
        public SanPham? SanPham { get; set; }
    }
}
