using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("KHUYENMAI")]
    public class KhuyenMai
    {
        [Key]
        [Column("MAKM")]
        [StringLength(20)]
        public string MaKM { get; set; }

        [Column("TENKM")]
        [StringLength(100)]
        public string? TenKM { get; set; }

        [Column("MOTA")]
        [Required]
        [StringLength(200)]
        public string? MoTa { get; set; }

        [Column("MALOAIKM")]
        [StringLength(20)]
        public string? MaLoaiKM { get; set; }

        [Column("PHANTRAMGIAM")]
        public int? PhanTramGiam { get; set; }

        //[Column("GIAMTOIDA")]
        //public int? GiamToiDa { get; set; }

        //[Column("GIAMTIEN")]
        //public int? GiamTien { get; set; }

        //[Column("DIEUKIEN")]
        //public int? DieuKien { get; set; } 

        [Column("NGAYBATDAU")]
        public DateTime? NgayBatDau { get; set; } 

        [Column("NGAYKETTHUC")]
        public DateTime? NgayKetThuc { get; set; }

        [Column("TRANGTHAI")]
        public int? TrangThai { get; set; }

        [Column("SOLUONG_APDUNG")]
        public int? SoLuongApDung { get; set; }

        [Column("SOLUONG_DA_APDUNG")]
        public int? SoLuongDaApDung { get; set; }


        // Navigation property (optional if you want to link LoaiKhuyenMai)
        [ForeignKey("MaLoaiKM")]
        public virtual LoaiKhuyenMai? LoaiKhuyenMai { get; set; }



    }
}
