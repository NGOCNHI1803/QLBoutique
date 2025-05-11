using QLBoutique.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LabManagement.Model
{
    public class SanPham
    {
        public int? MaSanPham { get; set; } // Mã sản phẩm

        public string? TenSanPham { get; set; } // Tên sản phẩm

        public double? GiaNhap { get; set; } // Giá nhập

        public double? GiaBan { get; set; } // Giá bán

        public int? SoLuongTon { get; set; } // Số lượng tồn

        public int? MaLoai { get; set; } // Mã loại sản phẩm

        public int? MaNCC { get; set; } // Mã nhà cung cấp

        public bool isDeleted { get; set; } = false;

        // Foreign key cho bảng LoaiSanPham
        [ForeignKey("MaLoai")]
        [JsonIgnore]
        public LoaiSanPham? LoaiSanPham { get; set; }

        // Foreign key cho bảng NhaCungCap
        [ForeignKey("MaNCC")]
        [JsonIgnore]
        public NhaCungCap? NhaCungCap { get; set; }
    }
}
