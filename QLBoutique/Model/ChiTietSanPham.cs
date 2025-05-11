using LabManagement.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class ChiTietSanPham
    {
        public int? MaChiTiet { get; set; } // Mã chi tiết sản phẩm

        public int? MaSanPham { get; set; } // Mã sản phẩm

        public int? MaLoaiSP { get; set; } // Mã loại sản phẩm

        public string? MaSKU { get; set; } // Mã SKU

        public string? MoTa { get; set; } // Mô tả

        public string? MauSac { get; set; } // Màu sắc

        public string? KichCo { get; set; } // Kích cỡ

        public string? ChatLieu { get; set; } // Chất liệu

        public string? DacTinh { get; set; } // Đặc tính

        public string? Form { get; set; } // Form dáng

        public int? Gia { get; set; } // Giá bán

        public int? SoLuong { get; set; } // Số lượng tồn kho

        public string? HinhAnh { get; set; } // Đường dẫn ảnh

        public bool isDeleted { get; set; } = false;

        [NotMapped]
        public string? HinhAnhUrl => string.IsNullOrEmpty(HinhAnh)
            ? null
            : $"https://localhost:7265/images/{HinhAnh}";

        // Navigation Property - liên kết với bảng SanPham
        [ForeignKey("MaSanPham")]
        [JsonIgnore]
        public SanPham? SanPham { get; set; }

        // Navigation Property - liên kết với bảng LoaiSanPham
        [ForeignKey("MaLoaiSP")]
        [JsonIgnore]
        public LoaiSanPham? LoaiSanPham { get; set; }
    }
}
