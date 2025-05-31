namespace QLBoutique.Model.DTO
{
    public class LoaiSanPhamDTO
    {
        public string? MaLoai { get; set; }
        public string? TenLoai { get; set; }
        public string? XuatSu { get; set; }
        public List<LoaiSanPhamDTO>? Children { get; set; }
    }
}
