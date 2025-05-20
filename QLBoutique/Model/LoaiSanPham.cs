using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class LoaiSanPham
    {
        public string? MaLoai { get; set; }      // MALOAI - khóa chính
        public string? TenLoai { get; set; }     // TENLOAI
        public string? XuatSu { get; set; }      // XUATSU
        public string? ParentId { get; set; }    // PARENT_ID - khóa ngoại (có thể null)

        [ForeignKey("ParentId")]
        [JsonIgnore]
        public LoaiSanPham? Parent { get; set; }

        // Có thể dùng List hoặc mảng
        public List<LoaiSanPham>? Children { get; set; }
    }
}
