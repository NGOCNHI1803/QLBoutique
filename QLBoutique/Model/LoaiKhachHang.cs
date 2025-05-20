using System.ComponentModel.DataAnnotations.Schema;

public class LoaiKhachHang
{
    [Column("MALOAIKH")]
    public string MaLoaiKH { get; set; }

    [Column("TENLOAIKH")]
    public string TenLoaiKH { get; set; }

    [Column("DIEUKIEN_TONGCHI")]
    public decimal? DieuKienTongChi { get; set; }

    [Column("PHANTRAMGIAM")]
    public int? PhanTramGiam { get; set; }

    [Column("MO_TA")]
    public string? MoTa { get; set; }
}
