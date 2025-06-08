using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("LICHSU_DIEM")]
    public class LichSuDiem
    {
        [Key]
        public int MaLSD { get; set; }

        [Column("MAKH")]
        [StringLength(20)]
        public string? MaKH { get; set; }

        [Column("NGAY")]
        public DateTime Ngay { get; set; }

        [Column("DIEM")]
        public int Diem { get; set; }

        [Column("LOAI")]
        [StringLength(20)]
        public string? Loai { get; set; }

        [Column("GHICHU")]
        [StringLength(200)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaKH")]
        [Required]
        public virtual KhachHang KhachHang { get; set; }
    }
}
