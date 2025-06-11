using System.Text.Json.Serialization;

namespace QLBoutique.Model
{
    public class ChucVu
    {
        public string MaCV { get; set; } = null!;    // varchar(20)
        public string? TenCV { get; set; } = null!;   // varchar(100)

        //[JsonIgnore]
        //public ICollection<NhanVien>? Nhanviens { get; set; }
    }
}
