namespace QLBoutique.Model
{
    public class NhanVien
    {
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
    }
}
