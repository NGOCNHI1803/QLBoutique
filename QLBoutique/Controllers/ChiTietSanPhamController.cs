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
    public class BienTheSanPhamController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        // Thư mục lưu hình (tương đối với root project, ví dụ: wwwroot/images/BienTheSanPham)
        private static readonly string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        // URL cơ sở để trả về client (thay bằng domain + port của bạn)
        private const string ImageBaseUrl = "https://localhost:7265/Images";

        public BienTheSanPhamController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/BienTheSanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetAll()
        {
            var list = await _context.ChiTietSanPham.AsNoTracking().ToListAsync();

            //// Đưa đường dẫn hình ảnh về dạng URL
            //list.ForEach(item =>
            //{
            //    if (!string.IsNullOrEmpty(item.HinhAnh))
            //    {
            //        string imagePath = Path.Combine(ImageDirectory, item.HinhAnh);
            //        if (System.IO.File.Exists(imagePath))
            //        {
            //            item.HinhAnh = $"{ImageBaseUrl}/{item.HinhAnh}";
            //        }
            //        else
            //        {
            //            item.HinhAnh = null; // hoặc giá trị mặc định nếu file không tồn tại
            //        }
            //    }
            //});

            return list;
        }

        // GET: api/BienTheSanPham/{mabienthe}
        [HttpGet("{mabienthe}")]
        public async Task<ActionResult<ChiTietSanPham>> GetById(string mabienthe)
        {
            var item = await _context.ChiTietSanPham.FindAsync(mabienthe);
            if (item == null)
                return NotFound();

            return item;
        }

        // POST: api/BienTheSanPham
        [HttpPost]
        public async Task<ActionResult<ChiTietSanPham>> Post(ChiTietSanPham newItem)
        {
            if (!string.IsNullOrEmpty(newItem.HinhAnh))
            {
                string fullImagePath = Path.Combine(ImageDirectory, newItem.HinhAnh);
                if (!System.IO.File.Exists(fullImagePath))
                    return BadRequest("Hình ảnh không tồn tại trong thư mục lưu trữ.");
            }

            bool exists = await _context.ChiTietSanPham.AnyAsync(c => c.MaBienThe == newItem.MaBienThe);
            if (exists)
                return Conflict("Mã biến thể đã tồn tại.");

            _context.ChiTietSanPham.Add(newItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { mabienthe = newItem.MaBienThe }, newItem);
        }

        // PUT: api/BienTheSanPham/{mabienthe}
        [HttpPut("{mabienthe}")]
        public async Task<IActionResult> Put(string mabienthe, ChiTietSanPham updatedItem)
        {
            if (mabienthe != updatedItem.MaBienThe)
                return BadRequest("Mã biến thể không khớp.");

            var existItem = await _context.ChiTietSanPham.FindAsync(mabienthe);
            if (existItem == null)
                return NotFound();

            if (!string.IsNullOrEmpty(updatedItem.HinhAnh))
            {
                string fullImagePath = Path.Combine(ImageDirectory, updatedItem.HinhAnh);
                if (!System.IO.File.Exists(fullImagePath))
                    return BadRequest("Hình ảnh không tồn tại trong thư mục lưu trữ.");
            }

            // Cập nhật tất cả thuộc tính hoặc từng thuộc tính cần thiết
            _context.Entry(existItem).CurrentValues.SetValues(updatedItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChiTietSanPhamExists(mabienthe))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/BienTheSanPham/{mabienthe}
        [HttpDelete("{mabienthe}")]
        public async Task<IActionResult> Delete(string mabienthe)
        {
            var item = await _context.ChiTietSanPham.FindAsync(mabienthe);
            if (item == null)
                return NotFound();

            _context.ChiTietSanPham.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ChiTietSanPhamExists(string mabienthe)
        {
            return await _context.ChiTietSanPham.AnyAsync(e => e.MaBienThe == mabienthe);
        }

        // Tìm kiếm theo mã sản phẩm MASP
        // GET: api/BienTheSanPham/search?masp=xxx
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> SearchByMaSP([FromQuery] string masp)
        {
            if (string.IsNullOrEmpty(masp))
                return BadRequest("Bạn phải cung cấp mã sản phẩm (masp).");

            var list = await _context.ChiTietSanPham
                .Where(c => c.MaSanPham == masp)
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
                return NotFound("Không tìm thấy biến thể cho sản phẩm này.");

            return list;
        }
    }
}
