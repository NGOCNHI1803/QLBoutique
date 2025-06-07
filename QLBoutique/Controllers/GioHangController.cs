using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
=======
using Microsoft.AspNetCore.Identity;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLBoutique.ClothingDbContext;
using LabManagement.Model;
>>>>>>> dbd1ab9 (Update backend)

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;
<<<<<<< HEAD

=======
>>>>>>> dbd1ab9 (Update backend)
        public GioHangController(BoutiqueDBContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        [HttpGet("khachhang/{maKH}")]
        public async Task<ActionResult<IEnumerable<GioHang>>> GetGioHangByKhachHang(string maKH)
        {
            var gioHangList = await _context.GioHang
                .Include(g => g.KhachHang)
                .Where(g => g.MaKhachHang == maKH)
                .ToListAsync();

            // Lọc giỏ hàng chỉ chứa các mục còn hiệu lực (TRANGTHAI = 1)
            var validGioHang = gioHangList.Where(g => g.TrangThai == 1).ToList();

            if (!validGioHang.Any())
            {
                // Trả về danh sách rỗng thay vì lỗi chuỗi text
                return Ok(new List<GioHang>());
            }

            return Ok(validGioHang);
        }


        // POST: api/GioHang
        [HttpPost]
        public async Task<ActionResult<GioHang>> PostGioHang(GioHang gioHang)
        {
            if (gioHang == null || string.IsNullOrEmpty(gioHang.MaGioHang))
            {
                return BadRequest("Thông tin giỏ hàng không hợp lệ.");
            }

            gioHang.NgayTao = DateTime.Now;
            gioHang.NgayCapNhat = DateTime.Now;
            gioHang.TrangThai = 1;

            _context.GioHang.Add(gioHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGioHangByKhachHang), new { maKH = gioHang.MaKhachHang }, gioHang);
        }

        // PUT: api/GioHang/{maGioHang}
        [HttpPut("{maGioHang}")]
        public async Task<IActionResult> PutGioHang(string maGioHang, GioHang updatedGioHang)
        {
            if (maGioHang != updatedGioHang.MaGioHang)
            {
                return BadRequest("Mã giỏ hàng không khớp.");
            }

            var gioHang = await _context.GioHang.FindAsync(maGioHang);
            if (gioHang == null)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }

            gioHang.MaKhachHang = updatedGioHang.MaKhachHang;
            gioHang.NgayCapNhat = DateTime.Now;
            gioHang.TrangThai = updatedGioHang.TrangThai;

            _context.Entry(gioHang).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GioHangExists(maGioHang))
                {
                    return NotFound("Giỏ hàng không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //// DELETE: api/GioHang/{maGioHang}
        //[HttpDelete("{maGioHang}")]
        //public async Task<IActionResult> DeleteGioHang(string maGioHang)
        //{
        //    var gioHang = await _context.GioHang.FindAsync(maGioHang);
        //    if (gioHang == null)
        //    {
        //        return NotFound("Không tìm thấy giỏ hàng.");
        //    }

        //    _context.GioHang.Remove(gioHang);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        // GET: api/GioHang/exist/{maGioHang}
        [HttpGet("exist/{maGioHang}")]
        public async Task<IActionResult> CheckExist(string maGioHang)
        {
            bool exists = await _context.GioHang.AnyAsync(g => g.MaGioHang == maGioHang);
            return Ok(exists);
        }

        private bool GioHangExists(string maGioHang)
        {
            return _context.GioHang.Any(e => e.MaGioHang == maGioHang);
        }
        // DELETE: api/GioHang/{maGioHang}
        [HttpDelete("{maGioHang}")]
        public async Task<IActionResult> DeleteGioHang(string maGioHang)
        {
            // Bước 1: Tìm giỏ hàng theo mã
            var gioHang = await _context.GioHang.FirstOrDefaultAsync(g => g.MaGioHang == maGioHang);
            if (gioHang == null)
            {
                return NotFound("Không tìm thấy giỏ hàng.");
            }

            try
            {
                // Bước 2: Lấy tất cả chi tiết giỏ hàng liên quan đến mã giỏ hàng
                var chiTietList = await _context.ChiTietGioHang
                    .Where(c => c.MaGioHang == maGioHang)
                    .ToListAsync();

                // Bước 3: Xóa toàn bộ chi tiết giỏ hàng trước
                if (chiTietList.Any())
                {
                    _context.ChiTietGioHang.RemoveRange(chiTietList);
                }

                // Bước 4: Xóa bản ghi giỏ hàng
                _context.GioHang.Remove(gioHang);

                // Bước 5: Lưu thay đổi
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa giỏ hàng: {ex.Message}");
            }
=======
        // GET: api/GioHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GioHang>>> GetAll()
        {
            return await _context.GioHang.ToListAsync();
        }

        // POST: api/GioHang
        [HttpPost]
        public async Task<ActionResult<GioHang>> AddGioHang([FromBody] GioHang gioHang)
        {
            if (gioHang == null)
            {
                return BadRequest("Dữ liệu giỏ hàng không hợp lệ.");
            }

            _context.GioHang.Add(gioHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = gioHang.MaGioHang }, gioHang);
>>>>>>> dbd1ab9 (Update backend)
        }


    }
<<<<<<< HEAD

}
=======
}
>>>>>>> dbd1ab9 (Update backend)
