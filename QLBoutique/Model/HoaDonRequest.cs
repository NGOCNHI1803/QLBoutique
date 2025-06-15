using QLBoutique.Model;

public class HoaDonRequest
{
    public string MaKH { get; set; }
    public string MaNV { get; set; }
    public DateTime NgayLap { get; set; }
    public decimal TongTien { get; set; }
    public decimal? GiamGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string? MaKM { get; set; }
    public string MaTT { get; set; }
    public int TrangThai { get; set; }
    public string? GhiChu { get; set; }

    public List<ChiTietHoaDonDTO> ChiTietHoaDon { get; set; }
}

public class ChiTietHoaDonDTO
{
    public string MaBienThe { get; set; }
    public int SoLuong { get; set; }
}

