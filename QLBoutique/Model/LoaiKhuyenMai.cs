using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("LOAI_KHUYENMAI")]
    public class LoaiKhuyenMai
    {
        [Column("MALOAIKM")]
        public string MaLoaiKM { get; set; }

        [Column("TENLOAIKM")]
        public string? TenLoaiKM { get; set; }

        [Column("GHICHU")]
        public string? GhiChu { get; set; }

    }
}
