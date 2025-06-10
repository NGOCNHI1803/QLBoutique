using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    public class NhanVien
    {

        public string MaNV { get; set; } = null!;         // NOT NULL
        public string? HoTen { get; set; }                 // NULL
        public DateTime? NgaySinh { get; set; }            // NULL
        public bool? GioiTinh { get; set; }                // NULL (bit)
        public string? DiaChi { get; set; }                // NULL
        public string? SDT { get; set; }                   // NULL
        public string? Email { get; set; }                 // NULL
        public DateTime? NgayVaoLam { get; set; }          // NULL
        public string Username { get; set; } = null!;      // NOT NULL
        public string Password { get; set; } = null!;      // NOT NULL
        public string? MaQuyen { get; set; }
        public virtual QuyenHan? Quyen { get; set; }
        public string? MaCV { get; set; }
        public virtual ChucVu? ChucVu { get; set; }
    }
}


