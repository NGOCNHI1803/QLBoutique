using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class GioHang
    {
        public string? MaGioHang { get; set; } // Mã giỏ hàng (MAGIOHANG)

        public string? MaKhachHang { get; set; } // Mã khách hàng (MAKH)

        public DateTime? NgayTao { get; set; }  // Ngày tạo (NGAYTAO)

        public DateTime? NgayCapNhat { get; set; } 
        public int TrangThai { get; set; } = 1; // 1: còn hiệu lực, 0: đã đặt hàng hoặc hủy

        // Navigation property tới bảng KhachHang
        [ForeignKey("MaKhachHang")]
        [JsonIgnore]
        public KhachHang? KhachHang { get; set; }
    }
}
