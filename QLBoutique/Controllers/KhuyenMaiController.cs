using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhuyenMaiController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public KhuyenMaiController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/KhuyenMai
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhuyenMai>>> GetAll()
        {
            var danhSach = await _context.KhuyenMai
    .Select(k => new {
        k.MaKM,
        k.TenKM,
        k.MoTa,
        k.MaLoaiKM,
        PhanTramGiam = k.PhanTramGiam ?? 0,
        TrangThai = k.TrangThai ?? 0,
        SoLuongApDung = k.SoLuongApDung ?? 0,
        SoLuongDaApDung = k.SoLuongDaApDung ?? 0,
        k.NgayBatDau,
        k.NgayKetThuc
    })
    .ToListAsync();

            return Ok(danhSach);
        }

        // GET: api/KhuyenMai/{maKM}
        [HttpGet("{maKM}")]
        public async Task<ActionResult<KhuyenMai>> GetKhuyenMaiById(string maKM)
        {
            var km = await _context.KhuyenMai.FindAsync(maKM);
            if (km == null)
                return NotFound();

            return Ok(km);
        }

        // POST: api/KhuyenMai
        [HttpPost]
        public async Task<IActionResult> AddKhuyenMai([FromBody] KhuyenMai newKhuyenMai)
        {
            if (newKhuyenMai == null)
            {
                return BadRequest("Dữ liệu khuyến mãi không hợp lệ");
            }

            if (string.IsNullOrWhiteSpace(newKhuyenMai.MaKM))
            {
                return BadRequest("Mã khuyến mãi không được để trống");
            }

            var exists = await _context.KhuyenMai.AnyAsync(k => k.MaKM == newKhuyenMai.MaKM);
            if (exists)
            {
                return Conflict("Mã khuyến mãi đã tồn tại");
            }

            _context.KhuyenMai.Add(newKhuyenMai);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKhuyenMaiById), new { maKM = newKhuyenMai.MaKM }, newKhuyenMai);
        }

        // PUT: api/KhuyenMai/{maKM}
        [HttpPut("{maKM}")]
        public async Task<IActionResult> UpdateKhuyenMai(string maKM, [FromBody] KhuyenMai updatedKhuyenMai)
        {
            if (maKM != updatedKhuyenMai.MaKM)
            {
                return BadRequest("Mã khuyến mãi không khớp.");
            }

            var existing = await _context.KhuyenMai.FindAsync(maKM);
            if (existing == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính
            existing.TenKM = updatedKhuyenMai.TenKM;
            existing.MoTa = updatedKhuyenMai.MoTa;
            existing.MaLoaiKM = updatedKhuyenMai.MaLoaiKM;
            existing.PhanTramGiam = updatedKhuyenMai.PhanTramGiam;
            existing.NgayBatDau = updatedKhuyenMai.NgayBatDau;
            existing.NgayKetThuc = updatedKhuyenMai.NgayKetThuc;
            existing.TrangThai = updatedKhuyenMai.TrangThai;
            existing.SoLuongApDung = updatedKhuyenMai.SoLuongApDung;
            existing.SoLuongDaApDung = updatedKhuyenMai.SoLuongDaApDung;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi cập nhật dữ liệu.");
            }

            return NoContent();
        }

        // DELETE: api/KhuyenMai/{maKM}
        [HttpDelete("{maKM}")]
        public async Task<IActionResult> DeleteKhuyenMai(string maKM)
        {
            var existing = await _context.KhuyenMai.FindAsync(maKM);
            if (existing == null)
            {
                return NotFound();
            }

            _context.KhuyenMai.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
