using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class PhieuNhap
    {
        [Key]
        [StringLength(20)]
        public string MaPhieuNhap { get; set; }

        [Required]
        [StringLength(20)]
        public string? MaNCC { get; set; }

        [Required]
        [StringLength(20)]
        public string? MaNV { get; set; }

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [StringLength(200)]
        public string? GhiChu { get; set; }

        public int TrangThai { get; set; } = 1;

        // Navigation property
        [JsonIgnore] // Không trả ra JSON
        [ForeignKey("MaNCC")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        [JsonIgnore]
        [ForeignKey("MaNV")]
        public virtual NhanVien? NhanVien { get; set; }

        [JsonIgnore]
        public virtual ICollection<ChiTietPhieuNhap>? ChiTietPhieuNhaps { get; set; }
    }
}
