using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class NhanVien
    {
        public uint MaNhanVien { get; set; } // Mã nhân viên

        public string? TenNV { get; set; } // Tên nhân viên

        public string? MatKhau { get; set; } // Mật khẩu

        public string? Email { get; set; } // Email

        public string? DiaChi { get; set; } // Địa chỉ

        public string? ChucVu { get; set; } // Chức vụ

        public DateTime NgayVaoLam { get; set; } // Ngày vào làm


        public bool isDeleted { get; set; } = false; // Đánh dấu xóa mềm
         //public uint MaAdmin { get; set; } // Mã admin phụ trách


        //// Navigation Property - liên kết với bảng Admin (nếu có)
        //[ForeignKey("MaAdmin")]
        //[JsonIgnore]
        //public Admin? Admin { get; set; }
    }
}
