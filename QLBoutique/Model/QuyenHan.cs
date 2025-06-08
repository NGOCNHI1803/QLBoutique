
namespace QLBoutique.Model
{
    public class QuyenHan
    {
        public string MaQuyen { get; set; }
        public string? TenQuyen { get; set; }
        public string? MoTa { get; set; }
        public ICollection<NhanVien>? Nhanviens { get; set; }
    }
}
