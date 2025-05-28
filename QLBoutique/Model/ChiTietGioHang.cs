using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class ChiTietGioHang
    {
        public string? MaGioHang { get; set; } // Mã giỏ hàng (MAGIOHANG)

        public string? MaBienThe { get; set; } // Mã biến thể sản phẩm (MABIEN_THE)

        public int SoLuong { get; set; } = 1; // Số lượng sản phẩm

        // Navigation property đến bảng GioHang
        [ForeignKey("MaGioHang")]
        [JsonIgnore]
        public GioHang? GioHang { get; set; }

        // Navigation property đến bảng ChiTietSanPham (BIEN_THE_SANPHAM)
        [ForeignKey("MaBienThe")]
        [JsonIgnore]
        public ChiTietSanPham? ChiTietSanPham { get; set; }
    }
}
