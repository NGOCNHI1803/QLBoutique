using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("DONVIVANCHUYEN")]
    public class DonViVanChuyen
    {
        [Key]
        [Column("MADVVC")]
        public string MaDVVC { get; set; }

        [Required]
        [Column("TENDVVC")]
        public string TenDVVC { get; set; }

        [Required]
        [Column("PHIVANCHUYEN")]
        public decimal PhiVanChuyen { get; set; }

        [Required]
        [Column("THOIGIAN_GIAO")]
        public string ThoiGianGiao { get; set; }
        public ICollection<HoaDon>HoaDons{ get; set; }

    }
}
