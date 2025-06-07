<<<<<<< HEAD
﻿namespace QLBoutique.Model
{
    public class QuyenHan
    {
        public string MaQuyen { get; set; } = null!;
        public string TenQuyen { get; set; } = null!;
        public string? MoTa { get; set; }
=======
namespace QLBoutique.Model
{
    public class QuyenHan
    {
        public string MaQuyen { get; set; }
        public string? TenQuyen { get; set; }
        public string? MoTa { get; set; }
        public ICollection<Nhanvien>? Nhanviens { get; set; }
>>>>>>> dbd1ab9 (Update backend)
    }
}
