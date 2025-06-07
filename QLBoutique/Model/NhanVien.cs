namespace QLBoutique.Model
{
    public class Nhanvien
    {
<<<<<<< HEAD
        public string MaNV { get; set; } = null!;
        public string? HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public bool? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? MaQuyen { get; set; }
        public string? MaCV { get; set; }

        public virtual QuyenHan? QuyenHan { get; set; }
        public virtual ChucVu? ChucVu { get; set; }
=======
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
        public QuyenHan? Quyen { get; set; }
        public string? MaCV { get; set; }
        public ChucVu? ChucVu { get; set; }
>>>>>>> dbd1ab9 (Update backend)
    }

}
