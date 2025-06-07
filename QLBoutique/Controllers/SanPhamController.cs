using LabManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;
        // Thư mục lưu hình (tương đối với root project, ví dụ: wwwroot/images/BienTheSanPham)
        private static readonly string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        // URL cơ sở để trả về client (thay bằng domain + port của bạn)
        private const string ImageBaseUrl = "https://localhost:7265/Images";

        public SanPhamController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetAll()
        {
            return await _context.SanPham
                .Where(sp => sp.TrangThai == 1)
                .ToListAsync();
        }

        // GET: api/SanPham/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetById(string id)
        {
            var sanPham = await _context.SanPham
                .FirstOrDefaultAsync(sp => sp.MaSanPham == id && sp.TrangThai == 1);

            if (sanPham == null)
            {
                return NotFound();
            }

            return sanPham;
        }

        // POST: api/SanPham
        [HttpPost]
        public async Task<ActionResult<SanPham>> Create(SanPham sanPham)
        {
            if (!string.IsNullOrEmpty(sanPham.HinhAnh))
            {
                string fullImagePath = Path.Combine(ImageDirectory, sanPham.HinhAnh);
                if (!System.IO.File.Exists(fullImagePath))
                {
                    return BadRequest("Hình ảnh không tồn tại.");
                }
            }
            sanPham.TrangThai = 1; // Mặc định là hoạt động khi tạo
            _context.SanPham.Add(sanPham);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = sanPham.MaSanPham }, sanPham);
        }

        // PUT: api/SanPham/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return BadRequest();
            }

            _context.Entry(sanPham).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanPhamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/SanPham/{id} (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham == null || sanPham.TrangThai == 0)
            {
                return NotFound();
            }

            sanPham.TrangThai = 0; // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SanPhamExists(string id)
        {
            return _context.SanPham.Any(sp => sp.MaSanPham == id && sp.TrangThai == 1);
        }

        // GET: api/SanPham/loai/{maLoaiSP}
        [HttpGet("loai/{maLoaiSP}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetSanPhamTheoLoaiSP(string maLoaiSP)
        {
            var danhSach = await _context.SanPham
                                         .Where(sp => sp.MaLoai == maLoaiSP && sp.TrangThai == 1)
                                         .ToListAsync();

            if (danhSach == null || !danhSach.Any())
            {
                return NotFound("Không tìm thấy sản phẩm thuộc loại này.");
            }

            return Ok(danhSach);
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file được gửi lên.");

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(ImageDirectory, fileName);

            Directory.CreateDirectory(ImageDirectory); // Đảm bảo thư mục tồn tại

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string imageUrl = $"{ImageBaseUrl}/{fileName}";
            return Ok(new { FileName = fileName, Url = imageUrl });
        }

        // Tìm kiếm theo tên sản phẩm
        // GET: api/SanPham/search?TenSP=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SanPham>>> SearchByTenSP([FromQuery] string tensp)
        {
            if (string.IsNullOrEmpty(tensp))
                return BadRequest("Bạn phải cung cấp tên sản phẩm (tensp).");

            var list = await _context.SanPham
                .Where(c => c.TenSanPham == tensp)
                .AsNoTracking()
                .ToListAsync();

            list.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.HinhAnh))
                {
                    string imagePath = Path.Combine(ImageDirectory, item.HinhAnh);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        item.HinhAnh = null;
                    }
                }
            });

            if (list.Count == 0)
                return NotFound("Không tìm thấy sản phẩm này.");

            return list;
        }


    }

}
