using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("DIACHI_KHACHHANG")] // nếu bảng trong CSDL có tên như vậy
    public class DiaChiKhachHang
    {
        [Key]
        [Column("MADIACHI")]
        [StringLength(20)]
        public string? MaDiaChi { get; set; }

        [Required]
        [Column("MAKH")]
        [StringLength(20)]
        public string? MaKH { get; set; }

        [Required]
        [Column("HOTEN_NGUOINHAN")]
        [StringLength(100)]
        public string? HoTenNguoiNhan { get; set; }

        [Required]
        [Column("SDT_NGUOINHAN")]
        [StringLength(15)]
        public string? SDTNguoiNhan { get; set; }

        [Required]
        [Column("DIACHI_CUTHE")]
        [StringLength(255)]
        public string? DiaChiCuThe { get; set; }

        [Required]
        [Column("TINH_TP")]
        [StringLength(100)]
        public string? TinhTP { get; set; }

        [Required]
        [Column("HUYEN_QUAN")]
        [StringLength(100)]
        public string? HuyenQuan { get; set; }

        [Required]
        [Column("XA_PHUONG")]
        [StringLength(100)]
        public string? XaPhuong { get; set; }

        [Column("MAC_DINH")]
        public bool MacDinh { get; set; }

        [Column("GHI_CHU")]
        [StringLength(200)]
        public string? GhiChu { get; set; }

        // Khóa ngoại
        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }

        public ICollection<HoaDon> HoaDons { get; set; }
    }
}
