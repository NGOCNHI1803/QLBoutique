using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLBoutique.Model
{
    [Table("PHUONGTHUC_THANHTOAN")]
    public class PhuongThucThanhToan
    {
        [Key]
        [Column("MATT")]
        [StringLength(20)]
        public string MaTT { get; set; }

        [Column("TENTT")]
        [StringLength(50)]
        public string TenTT { get; set; }
    }
}
