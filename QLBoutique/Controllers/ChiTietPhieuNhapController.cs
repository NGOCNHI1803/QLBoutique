using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChiTietPhieuNhapController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public ChiTietPhieuNhapController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/ChiTietPhieuNhap/{maphieunhap}
        [HttpGet("{maphieunhap}")]
        public async Task<ActionResult<IEnumerable<ChiTietPhieuNhap>>> GetChiTietPhieuNhaps(string maphieunhap)
        {
            var chiTietList = await _context.ChiTietPhieuNhap
                .Where(ct => ct.MaPhieuNhap == maphieunhap)
                .Include(ct => ct.BienTheSanPham)
                .ToListAsync();

            if (chiTietList == null || chiTietList.Count == 0)
                return NotFound();

            return chiTietList;
        }

        // GET: api/ChiTietPhieuNhap/{maphieunhap}/{mabienthe}
        [HttpGet("{maphieunhap}/{mabienthe}")]
        public async Task<ActionResult<ChiTietPhieuNhap>> GetChiTietPhieuNhap(string maphieunhap, string mabienthe)
        {
            var chiTiet = await _context.ChiTietPhieuNhap
                .Include(ct => ct.BienTheSanPham)
                .FirstOrDefaultAsync(ct => ct.MaPhieuNhap == maphieunhap && ct.MaBienThe == mabienthe);

            if (chiTiet == null)
                return NotFound();

            return chiTiet;
        }

        // POST: api/ChiTietPhieuNhap
        //[HttpPost]
        //public async Task<ActionResult<ChiTietPhieuNhap>> CreateChiTietPhieuNhap([FromBody] ChiTietPhieuNhap chiTiet)
        //{
        //    _context.ChiTietPhieuNhaps.Add(chiTiet);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Có thể lỗi trùng khóa chính hoặc FK
        //        return BadRequest(new { message = ex.Message });
        //    }

        //    return CreatedAtAction(nameof(GetChiTietPhieuNhap),
        //        new { maphieunhap = chiTiet.MaPhieuNhap, mabienthe = chiTiet.MaBienThe }, chiTiet);
        //}
        [HttpPost]
        public async Task<ActionResult<ChiTietPhieuNhap>> CreateChiTietPhieuNhap([FromBody] ChiTietPhieuNhap chiTiet)
        {
            // Kiểm tra biến thể sản phẩm tồn tại không
            var bienThe = await _context.ChiTietSanPham
                .FirstOrDefaultAsync(bt => bt.MaBienThe == chiTiet.MaBienThe);

            if (bienThe == null)
            {
                return BadRequest(new { message = "Biến thể sản phẩm không tồn tại." });
            }

            // Cập nhật số lượng tồn kho trong biến thể sản phẩm
            bienThe.TonKho += chiTiet.SoLuong;

            // Thêm chi tiết phiếu nhập
            _context.ChiTietPhieuNhap.Add(chiTiet);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return CreatedAtAction(nameof(GetChiTietPhieuNhap),
                new { maphieunhap = chiTiet.MaPhieuNhap, mabienthe = chiTiet.MaBienThe }, chiTiet);
        }


        // PUT: api/ChiTietPhieuNhap/{maphieunhap}/{mabienthe}
        [HttpPut("{maphieunhap}/{mabienthe}")]
        public async Task<IActionResult> UpdateChiTietPhieuNhap(string maphieunhap, string mabienthe, [FromBody] ChiTietPhieuNhap chiTiet)
        {
            if (maphieunhap != chiTiet.MaPhieuNhap || mabienthe != chiTiet.MaBienThe)
                return BadRequest("Khóa chính không khớp.");

            var existingChiTiet = await _context.ChiTietPhieuNhap
                .FirstOrDefaultAsync(ct => ct.MaPhieuNhap == maphieunhap && ct.MaBienThe == mabienthe);

            if (existingChiTiet == null)
                return NotFound();

            existingChiTiet.SoLuong = chiTiet.SoLuong;
            existingChiTiet.Gia_Von = chiTiet.Gia_Von;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChiTietPhieuNhapExists(maphieunhap, mabienthe))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ChiTietPhieuNhap/{maphieunhap}/{mabienthe}
        [HttpDelete("{maphieunhap}/{mabienthe}")]
        public async Task<IActionResult> DeleteChiTietPhieuNhap(string maphieunhap, string mabienthe)
        {
            var chiTiet = await _context.ChiTietPhieuNhap
                .FirstOrDefaultAsync(ct => ct.MaPhieuNhap == maphieunhap && ct.MaBienThe == mabienthe);

            if (chiTiet == null)
                return NotFound();

            _context.ChiTietPhieuNhap.Remove(chiTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChiTietPhieuNhapExists(string maphieunhap, string mabienthe)
        {
            return _context.ChiTietPhieuNhap.Any(ct => ct.MaPhieuNhap == maphieunhap && ct.MaBienThe == mabienthe);
        }
    }
}
