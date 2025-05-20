using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiKHController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public LoaiKHController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/LoaiKH
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiKhachHang>>> GetAll()
        {
            return await _context.LoaiKhachHang.ToListAsync();
        }

        // GET: api/LoaiKH/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiKhachHang>> GetById(string id)
        {
            var loai = await _context.LoaiKhachHang.FindAsync(id);

            if (loai == null)
            {
                return NotFound();
            }

            return loai;
        }

        // POST: api/LoaiKH
        [HttpPost]
        public async Task<ActionResult<LoaiKhachHang>> Create(LoaiKhachHang loaiKhachHang)
        {
            if (await _context.LoaiKhachHang.AnyAsync(l => l.MaLoaiKH == loaiKhachHang.MaLoaiKH))
            {
                return BadRequest("Mã loại khách hàng đã tồn tại.");
            }

            _context.LoaiKhachHang.Add(loaiKhachHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = loaiKhachHang.MaLoaiKH }, loaiKhachHang);
        }

        // PUT: api/LoaiKH/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, LoaiKhachHang loaiKhachHang)
        {
            if (id != loaiKhachHang.MaLoaiKH)
            {
                return BadRequest("Mã không khớp.");
            }

            var existing = await _context.LoaiKhachHang.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.TenLoaiKH = loaiKhachHang.TenLoaiKH;
            existing.DieuKienTongChi = loaiKhachHang.DieuKienTongChi;
            existing.PhanTramGiam = loaiKhachHang.PhanTramGiam;
            existing.MoTa = loaiKhachHang.MoTa;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/LoaiKH/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var loai = await _context.LoaiKhachHang.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }

            _context.LoaiKhachHang.Remove(loai);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
