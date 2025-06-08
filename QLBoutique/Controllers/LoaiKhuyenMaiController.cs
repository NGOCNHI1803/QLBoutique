using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiKhuyenMaiController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public LoaiKhuyenMaiController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/LoaiKM
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiKhuyenMai>>> GetAll()
        {
            return await _context.LoaiKhuyenMai.ToListAsync();
        }

        // PUT: api/LoaiKhuyenMai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, LoaiKhuyenMai loaiKhuyenMai)
        {
            if (id != loaiKhuyenMai.MaLoaiKM)
            {
                return BadRequest("Mã loại khuyến mãi không khớp.");
            }

            var existing = await _context.LoaiKhuyenMai.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính
            existing.TenLoaiKM = loaiKhuyenMai.TenLoaiKM;
            // Nếu có các thuộc tính khác, cập nhật thêm ở đây

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý khi xảy ra lỗi đồng bộ dữ liệu (nếu cần)
                throw;
            }

            return NoContent();
        }

        // DELETE: api/LoaiKhuyenMai/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var loai = await _context.LoaiKhuyenMai.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }

            _context.LoaiKhuyenMai.Remove(loai);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
