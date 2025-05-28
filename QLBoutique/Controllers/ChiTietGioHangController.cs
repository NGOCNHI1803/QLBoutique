using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;

namespace QLBoutique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietGioHangController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public ChiTietGioHangController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/ChiTietGioHang/{maGioHang}
        [HttpGet("{maGioHang}")]
        public async Task<ActionResult<IEnumerable<ChiTietGioHang>>> GetCartItems(string maGioHang)
        {
            var gioHang = await _context.GioHang.FirstOrDefaultAsync(g => g.MaGioHang == maGioHang);

            if (gioHang == null)
                return NotFound("Giỏ hàng không tồn tại");

            var chiTiet = await _context.ChiTietGioHang
                .Where(c => c.MaGioHang == maGioHang)
                .ToListAsync();

            if (chiTiet == null || chiTiet.Count == 0)
                return Ok("Giỏ hàng trống");

            // Xử lý sản phẩm hết hiệu lực hoặc đã đặt
            var chiTietHieuLuc = new List<ChiTietGioHang>();

            foreach (var item in chiTiet)
            {
                var bienThe = await _context.ChiTietSanPham
                    .FirstOrDefaultAsync(b => b.MaBienThe == item.MaBienThe && b.TrangThai == 1);

                if (bienThe != null)
                {
                    chiTietHieuLuc.Add(item);
                }
                else
                {
                    // Xóa item không còn hiệu lực
                    _context.ChiTietGioHang.Remove(item);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(chiTietHieuLuc);
        }

        // POST: api/ChiTietGioHang
        [HttpPost]
        public async Task<ActionResult> AddToCart(ChiTietGioHang item)
        {
            if (item == null || item.MaGioHang == null || item.MaBienThe == null || item.SoLuong <= 0)
                return BadRequest("Thông tin không hợp lệ");

            var bienThe = await _context.ChiTietSanPham.FindAsync(item.MaBienThe);
            if (bienThe == null || bienThe.TrangThai != 1)
                return BadRequest("Sản phẩm không tồn tại hoặc không còn hiệu lực");

            if (bienThe.TonKho < item.SoLuong)
                return BadRequest("Không đủ hàng trong kho");

            var existingItem = await _context.ChiTietGioHang
                .FirstOrDefaultAsync(c => c.MaGioHang == item.MaGioHang && c.MaBienThe == item.MaBienThe);

            if (existingItem != null)
            {
                int tongSoLuong = existingItem.SoLuong + item.SoLuong;
                if (tongSoLuong > bienThe.TonKho)
                    return BadRequest("Số lượng vượt quá tồn kho");

                existingItem.SoLuong = tongSoLuong;
            }
            else
            {
                _context.ChiTietGioHang.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok("Thêm vào giỏ hàng thành công");
        }

        // PUT: api/ChiTietGioHang
        [HttpPut]
        public async Task<ActionResult> UpdateQuantity(ChiTietGioHang item)
        {
            if (item == null || item.SoLuong <= 0)
                return BadRequest("Số lượng không hợp lệ");

            var gioHangItem = await _context.ChiTietGioHang
                .FirstOrDefaultAsync(c => c.MaGioHang == item.MaGioHang && c.MaBienThe == item.MaBienThe);

            if (gioHangItem == null)
                return NotFound("Không tìm thấy sản phẩm trong giỏ");

            var bienThe = await _context.ChiTietSanPham.FindAsync(item.MaBienThe);
            if (bienThe == null || item.SoLuong > bienThe.TonKho)
                return BadRequest("Số lượng vượt quá tồn kho");

            gioHangItem.SoLuong = item.SoLuong;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật số lượng thành công");
        }

        // DELETE: api/ChiTietGioHang?maGioHang=xxx&maBienThe=yyy
        [HttpDelete]
        public async Task<ActionResult> RemoveFromCart([FromQuery] string maGioHang, [FromQuery] string maBienThe)
        {
            var gioHangItem = await _context.ChiTietGioHang
                .FirstOrDefaultAsync(c => c.MaGioHang == maGioHang && c.MaBienThe == maBienThe);

            if (gioHangItem == null)
                return NotFound("Không tìm thấy sản phẩm trong giỏ");

            _context.ChiTietGioHang.Remove(gioHangItem);
            await _context.SaveChangesAsync();
            return Ok("Xóa sản phẩm khỏi giỏ hàng thành công");
        }
    }
}
