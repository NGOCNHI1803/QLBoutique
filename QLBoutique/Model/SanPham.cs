using QLBoutique.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LabManagement.Model
{
    public class SanPham
    {
        public string? MaSanPham { get; set; } // Mã sản phẩm (MASP)

        public string? TenSanPham { get; set; } // Tên sản phẩm (TENSP)

        public string? MoTa { get; set; } // Mô tả (MOTA)

        public string? HinhAnh { get; set; } // Hình ảnh (HINHANH)

        public string? MaLoai { get; set; } // Mã loại sản phẩm (MALOAI)

        public string? MaNCC { get; set; } // Mã nhà cung cấp (MANCC)

        public int TrangThai { get; set; } = 1; // 1 hoạt động, 0 ngừng

        [NotMapped]
        public string? HinhAnhUrl => string.IsNullOrEmpty(HinhAnh)
            ? null
            : $"https://localhost:7265/images/{HinhAnh}";

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
