<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
=======
﻿using System;
using System.ComponentModel.DataAnnotations;
>>>>>>> 55807bd3bafc46dd83db3d3e7badd936e740ced9
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    [Table("CHITIET_HOADON")]
    public class ChiTietHoaDon
    {
        [Key]
        [Column("MACHITIET_HD")]
<<<<<<< HEAD
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
=======
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
>>>>>>> 55807bd3bafc46dd83db3d3e7badd936e740ced9

        [Column("THANHTIEN")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaHD")]
<<<<<<< HEAD
        [JsonIgnore]
        public HoaDon? HoaDon { get; set; }

        [ForeignKey("MaBienThe")]
        [JsonIgnore]
        public ChiTietSanPham? ChiTietSanPham { get; set; }
=======
        [JsonIgnore] // Ignore vòng tham chiếu về HoaDon khi serialize
        public HoaDon HoaDon { get; set; }

        [ForeignKey("MaBienThe")]
        public ChiTietSanPham BienTheSanPham { get; set; }
>>>>>>> 55807bd3bafc46dd83db3d3e7badd936e740ced9
    }
}
