using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace QLBoutique.Model
{
    
    public class NhaCungCap
    {
        [Key]
        [Column("MaNCC")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng (AUTO_INCREMENT)
        public int MaNCC { get; set; }

        [Required]
        [Column("TenNCC")]
        [StringLength(150)]
        public string TenNCC { get; set; } = null!;

        [Required]
        [Column("ThongTinLienHe")]
        [StringLength(150)]
        public string ThongTinLienHe { get; set; } = null!;
    }
}
