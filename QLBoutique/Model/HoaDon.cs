using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    [Table("HOADON")]
    public class HoaDon
    {
        [Key]
        [StringLength(20)]
        public string MaHD { get; set; }

        [StringLength(20)]
        public string MaKH { get; set; }

        [Required]
        [StringLength(20)]
        public string MaNV { get; set; }

        public DateTime NgayLap { get; set; } = DateTime.Now;

        public decimal TongTien { get; set; }

        public decimal GiamGia { get; set; }

        public decimal ThanhTien { get; set; }

        [StringLength(20)]
        public string? MaKM { get; set; }

        public int TrangThai { get; set; } = 1;

        [StringLength(200)]
        public string GhiChu { get; set; }

        [StringLength(20)]
        public string MaDiaChi { get; set; }

        [Required]
        [StringLength(20)]
        public string MaTT { get; set; }

        [Required]
        [StringLength(20)]
        public string MaDVVC { get; set; }

        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien NhanVien { get; set; }

        [ForeignKey("MaKM")]
        [JsonIgnore]
        public KhuyenMai? KhuyenMai { get; set; }

        [ForeignKey("MaTT")]
        [InverseProperty("HoaDons")]
        [JsonIgnore]
        public PhuongThucThanhToan? PhuongThucThanhToan { get; set; }

        [ForeignKey("MaDiaChi")]
        public DiaChiKhachHang DiaChiKhachHang { get; set; }

        [ForeignKey("MaDVVC")]
        public DonViVanChuyen DonViVanChuyen { get; set; }

        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
