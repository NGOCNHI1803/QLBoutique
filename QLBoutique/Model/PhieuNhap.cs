using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Điều hướng (Navigation properties)
        [ForeignKey("MaNCC")]
        public virtual NhaCungCap? NhaCungCap { get; set; }

        [ForeignKey("MaNV")]
        public virtual NhanVien? NhanVien { get; set; }

        public virtual ICollection<ChiTietPhieuNhap>? ChiTietPhieuNhaps { get; set; }
    }
}
