using LabManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietSanPhamController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        private static readonly string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "ChiTietSanPham");
        private const string ImageBaseUrl = "https://localhost:7265/images";

        public ChiTietSanPhamController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/ChiTietSanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetChiTietSanPhams()
        {
            return await _context.ChiTietSanPham
                                 .Where(sp => !sp.isDeleted)
                                 .ToListAsync();
        }

        // GET: api/ChiTietSanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietSanPham>> GetChiTietSanPham(int id)
        {
            var sp = await _context.ChiTietSanPham.FindAsync(id);

            if (sp == null || sp.isDeleted)
            {
                return NotFound();
            }

            return sp;
        }

        // POST: api/ChiTietSanPham
        [HttpPost]
        public async Task<ActionResult<ChiTietSanPham>> PostChiTietSanPham([FromBody] ChiTietSanPham chiTiet)
        {
            _context.ChiTietSanPham.Add(chiTiet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChiTietSanPham), new { id = chiTiet.MaChiTiet }, chiTiet);
        }

        // PUT: api/ChiTietSanPham/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChiTietSanPham(int id, [FromBody] ChiTietSanPham chiTiet)
        {
            if (id != chiTiet.MaChiTiet)
                return BadRequest();

            var existing = await _context.ChiTietSanPham.FindAsync(id);
            if (existing == null || existing.isDeleted)
                return NotFound();

            // Update fields
            _context.Entry(existing).CurrentValues.SetValues(chiTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ChiTietSanPham/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChiTietSanPham(int id)
        {
            var sp = await _context.ChiTietSanPham.FindAsync(id);
            if (sp == null || sp.isDeleted)
                return NotFound();

            sp.isDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Upload hình ảnh
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            if (!Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(ImageDirectory);
            }

            var fileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(ImageDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{ImageBaseUrl}/{fileName}";
            return Ok(new { imageUrl, fileName });
        }

        // GET: api/ChiTietSanPham/loai/{maLoaiSP}
        [HttpGet("loai/{maLoaiSP}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetByMaLoaiSP(int maLoaiSP)
        {
            var list = await _context.ChiTietSanPham
                                     .Where(ct => ct.MaLoaiSP == maLoaiSP && !ct.isDeleted)
                                     .ToListAsync();

            if (list == null || list.Count == 0)
            {
                return NotFound("Không tìm thấy sản phẩm với mã loại đã cho.");
            }

            return Ok(list);
        }
        // GET: api/ChiTietSanPham/sanpham/{maSanPham}
        [HttpGet("sanpham/{maSanPham}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetChiTietTheoMaSanPham(int maSanPham)
        {
            var danhSachChiTiet = await _context.ChiTietSanPham
                .Where(ct => ct.MaSanPham == maSanPham && !ct.isDeleted)
                .ToListAsync();

            if (danhSachChiTiet == null || danhSachChiTiet.Count == 0)
            {
                return NotFound("Không tìm thấy chi tiết sản phẩm với mã sản phẩm đã cho.");
            }

            return Ok(danhSachChiTiet);
        }


    }
}
