using LabManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuNhapController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public PhieuNhapController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/PhieuNhap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuNhap>>> GetPhieuNhaps()
        {
            return await _context.PhieuNhaps
                .Include(p => p.NhaCungCap)
                .Include(p => p.NhanVien)
                .Include(p => p.ChiTietPhieuNhaps)
                .ToListAsync();
        }

        // GET: api/PhieuNhap/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuNhap>> GetPhieuNhap(string id)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .ThenInclude(ct => ct.BienTheSanPham)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
            {
                return NotFound();
            }

            return phieuNhap;
        }

        // POST: api/PhieuNhap
        [HttpPost]
        public async Task<ActionResult<PhieuNhap>> CreatePhieuNhap([FromBody] PhieuNhap phieuNhap)
        {
            // Tính tổng tiền dựa trên chi tiết nhập
            if (phieuNhap.ChiTietPhieuNhaps != null)
            {
                phieuNhap.TongTien = phieuNhap.ChiTietPhieuNhaps.Sum(ct => ct.SoLuong * ct.Gia_Von);
            }

            phieuNhap.NgayNhap = DateTime.Now;
            _context.PhieuNhaps.Add(phieuNhap);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Bạn có thể ghi log lỗi ở đây
                return BadRequest(new { message = ex.Message });
            }

            return CreatedAtAction(nameof(GetPhieuNhap), new { id = phieuNhap.MaPhieuNhap }, phieuNhap);
        }

        // PUT: api/PhieuNhap/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhieuNhap(string id, [FromBody] PhieuNhap phieuNhap)
        {
            if (id != phieuNhap.MaPhieuNhap)
            {
                return BadRequest("Mã phiếu nhập không khớp.");
            }

            var existingPhieuNhap = await _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (existingPhieuNhap == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin phiếu nhập
            existingPhieuNhap.MaNCC = phieuNhap.MaNCC;
            existingPhieuNhap.MaNV = phieuNhap.MaNV;
            existingPhieuNhap.GhiChu = phieuNhap.GhiChu;
            existingPhieuNhap.TrangThai = phieuNhap.TrangThai;

            // Xóa chi tiết cũ
            _context.ChiTietPhieuNhaps.RemoveRange(existingPhieuNhap.ChiTietPhieuNhaps);

            // Thêm chi tiết mới
            if (phieuNhap.ChiTietPhieuNhaps != null)
            {
                foreach (var ct in phieuNhap.ChiTietPhieuNhaps)
                {
                    ct.MaPhieuNhap = id;
                    _context.ChiTietPhieuNhaps.Add(ct);
                }

                existingPhieuNhap.TongTien = phieuNhap.ChiTietPhieuNhaps.Sum(ct => ct.SoLuong * ct.Gia_Von);
            }
            else
            {
                existingPhieuNhap.TongTien = 0;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PhieuNhaps.Any(e => e.MaPhieuNhap == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/PhieuNhap/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuNhap(string id)
        {
            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == id);

            if (phieuNhap == null)
            {
                return NotFound();
            }

            // Xóa chi tiết trước để tránh FK violation
            if (phieuNhap.ChiTietPhieuNhaps != null)
            {
                _context.ChiTietPhieuNhaps.RemoveRange(phieuNhap.ChiTietPhieuNhaps);
            }

            _context.PhieuNhaps.Remove(phieuNhap);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
