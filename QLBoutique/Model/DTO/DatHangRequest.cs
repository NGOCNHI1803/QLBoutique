namespace QLBoutique.Model.DTO
{
    public class DatHangRequest
    {
        public string MaKH { get; set; }
        public string? MaNV { get; set; }  // nếu có
        public string MaDiaChi { get; set; }
        public string MaDVVC { get; set; }
        public string? MaTT { get; set; }
        public string? MaKM { get; set; }
        public string? GhiChu { get; set; }

        public List<SanPhamDatHangDto> SanPhams { get; set; }
    }

    public class SanPhamDatHangDto
    {
        public string MaBienThe { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiamGia { get; set; }
    }

}
