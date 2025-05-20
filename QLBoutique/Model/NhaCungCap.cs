namespace QLBoutique.Model
{
    public class NhaCungCap
    {
        public string? MaNCC { get; set; }       // MANCC - khóa chính
        public string? TenNCC { get; set; }      // TENNCC
        public string DiaChi { get; set; } = ""; // DIACHI - NOT NULL nên cần khởi tạo
        public string? SDT { get; set; }         // SDT - có thể null
        public string? Email { get; set; }       // EMAIL - có thể null
        public int TrangThai { get; set; } = 1;  // TRANGTHAI - mặc định là 1
    }
}
