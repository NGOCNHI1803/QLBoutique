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
                return Ok(new { message = "Giỏ hàng trống", data = new List<ChiTietGioHang>() });


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
        // GET: api/ChiTietGioHang/ma-bien-the?maSanPham=SP001&mauSac=Đỏ&kichThuoc=M
        [HttpGet("ma-bien-the")]
        public async Task<ActionResult<string>> GetMaBienThe([FromQuery] string maSanPham, [FromQuery] string mauSac, [FromQuery] string kichThuoc)
        {
            if (string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(mauSac) || string.IsNullOrEmpty(kichThuoc))
                return BadRequest("Thiếu thông tin cần thiết");

            var bienThe = await _context.ChiTietSanPham
                .FirstOrDefaultAsync(bt =>
                    bt.MaSanPham == maSanPham &&
                    bt.MauSac.ToLower() == mauSac &&
                    bt.Size.ToLower() == kichThuoc &&
                    bt.TrangThai == 1);

            if (bienThe == null)
                return NotFound("Không tìm thấy biến thể phù hợp");

            return Ok(new { maBienThe = bienThe.MaBienThe });
        }

        // GET: api/ChiTietGioHang/bienthe/{maBienThe}
        [HttpGet("bienthe/{maBienThe}")]
        public async Task<ActionResult<ChiTietSanPham>> GetBienTheTheoMaBienThe(string maBienThe)
        {
            if (string.IsNullOrEmpty(maBienThe))
                return BadRequest("Mã biến thể không được để trống");

            var bienThe = await _context.ChiTietSanPham
                .Include(bt => bt.SanPham) // nếu muốn lấy luôn thông tin sản phẩm cha (tuỳ thuộc model của bạn)
                .FirstOrDefaultAsync(bt => bt.MaBienThe == maBienThe && bt.TrangThai == 1);

            if (bienThe == null)
                return NotFound("Không tìm thấy biến thể sản phẩm");

            return Ok(bienThe);
        }
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.MaKhachHang) || string.IsNullOrEmpty(request.MaBienThe) || request.SoLuong < 1)
                return BadRequest("Dữ liệu không hợp lệ.");

            try
            {
                // Bước 1: Tìm giỏ hàng còn hiệu lực của khách hàng
                var gioHang = await _context.GioHang
                    .FirstOrDefaultAsync(g => g.MaKH == request.MaKhachHang && g.TrangThai == 1);

                // Bước 2: Nếu chưa có giỏ thì tạo mới
                if (gioHang == null)
                {
                    gioHang = new GioHang
                    {
                        MaGioHang = Guid.NewGuid().ToString("N").Substring(0, 20), // tạo mã 20 ký tự
                        MaKH = request.MaKhachHang,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now,
                        TrangThai = 1
                    };

                    _context.GioHang.Add(gioHang);
                    await _context.SaveChangesAsync(); // Lưu để có mã giỏ hàng
                }

                // Bước 3: Kiểm tra biến thể sản phẩm đã có trong giỏ chưa
                var chiTiet = await _context.ChiTietGioHang
                    .FirstOrDefaultAsync(ct => ct.MaGioHang == gioHang.MaGioHang && ct.MaBienThe == request.MaBienThe);

                if (chiTiet != null)
                {
                    // Cập nhật số lượng cộng dồn
                    chiTiet.SoLuong += request.SoLuong;
                    _context.Entry(chiTiet).State = EntityState.Modified;
                }
                else
                {
                    // Thêm mới chi tiết giỏ hàng
                    chiTiet = new ChiTietGioHang
                    {
                        MaGioHang = gioHang.MaGioHang,
                        MaBienThe = request.MaBienThe,
                        SoLuong = request.SoLuong
                    };
                    _context.ChiTietGioHang.Add(chiTiet);
                }

                // Cập nhật lại ngày cập nhật giỏ hàng
                gioHang.NgayCapNhat = DateTime.Now;
                _context.Entry(gioHang).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Thêm sản phẩm vào giỏ hàng thành công",
                    MaGioHang = gioHang.MaGioHang,
                    ChiTiet = chiTiet
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi thêm sản phẩm vào giỏ hàng: {ex.Message}");
            }
        }

        // Model nhận request
        public class AddToCartRequest
        {
            public string? MaKhachHang { get; set; }
            public string? MaBienThe { get; set; }
            public int SoLuong { get; set; }
        }


    }
}
