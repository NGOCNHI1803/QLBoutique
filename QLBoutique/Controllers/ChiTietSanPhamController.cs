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
        // GET: api/BienTheSanPham/{maSanPham}
        [HttpGet("GetByProductId/{masanpham}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetByIdProduct(string masanpham)
        {
            var list = await _context.ChiTietSanPham
                .Where(ct => ct.MaSanPham == masanpham)
                .ToListAsync();

            //if (list == null || list.Count == 0)
            //    return NotFound("Không tìm thấy biến thể nào cho sản phẩm này.");

            return list;
        }
        // GET: api/BienTheSanPham/{Barcode}
        [HttpGet("GetByBarcode/{barcode}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetByBarcode(string barcode)
        {
            var list = await _context.ChiTietSanPham
                .Where(ct => ct.Barcode.Trim() == barcode.Trim())
                .ToListAsync();

            if (list == null || list.Count == 0)
                return NotFound("Không tìm thấy biến thể nào cho sản phẩm này.");

            return list;
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
        // POST: api/BienTheSanPham/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file được gửi lên.");

            // Tạo thư mục lưu ảnh nếu chưa có
            if (!Directory.Exists(ImageDirectory))
                Directory.CreateDirectory(ImageDirectory);

            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(ImageDirectory, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{ImageBaseUrl}/{fileName}";
            return Ok(new { imageUrl, fileName });
        }
        // GET: api/BienTheSanPham/loai/{maLoaiSP}
        [HttpGet("loai/{maLoaiSP}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetByMaLoaiSP(string maLoaiSP)
        {
            var result = await _context.ChiTietSanPham
                .Include(ct => ct.SanPham) // bạn cần đảm bảo có navigation property từ ChiTietSanPham đến SanPham
                .Where(ct => ct.SanPham.MaLoai == maLoaiSP)
                .AsNoTracking()
                .ToListAsync();

            if (result == null || result.Count == 0)
                return NotFound($"Không tìm thấy sản phẩm nào thuộc mã loại {maLoaiSP}");

            return result;
        }
        // GET: api/BienTheSanPham/sanpham/{maSP}
        [HttpGet("sanpham/{maSP}")]
        public async Task<ActionResult<IEnumerable<ChiTietSanPham>>> GetBienTheTheoMaSP(string maSP)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                return BadRequest("Mã sản phẩm không được để trống.");

            var bienTheList = await _context.ChiTietSanPham
                .Where(c => c.MaSanPham == maSP)
                .AsNoTracking()
                .ToListAsync();

            if (bienTheList == null || bienTheList.Count == 0)
                return NotFound($"Không tìm thấy biến thể nào cho mã sản phẩm: {maSP}");

            // Kiểm tra và xử lý đường dẫn hình ảnh
            bienTheList.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.HinhAnh))
                {
                    string imagePath = Path.Combine(ImageDirectory, item.HinhAnh);
                    if (!System.IO.File.Exists(imagePath))
                    {
                        item.HinhAnh = null; // hoặc thay bằng hình mặc định
                    }
                    else
                    {
                        item.HinhAnh = $"{ImageBaseUrl}/{item.HinhAnh}";
                    }
                }
            });

            return bienTheList;
        }

        [HttpGet("TimMaBienThe")]
        public IActionResult TimMaBienThe(string tenSP, string size, string mauSac)
        {
            var sanPham = _context.SanPham.FirstOrDefault(sp => sp.TenSanPham == tenSP);
            if (sanPham == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm theo tên." });
            }

            string maSP = sanPham.MaSanPham;

            var bienThe = _context.ChiTietSanPham.FirstOrDefault(bt =>
                bt.MaSanPham == maSP &&
                bt.Size == size &&
                bt.MauSac == mauSac
            );

            if (bienThe == null)
            {
                return NotFound(new { message = "Không tìm thấy biến thể." });
            }

            return Ok(new { MaBienThe = bienThe.MaBienThe });
        }

        // GET: api/ChiTietSanPham/tonkho?maSP=ABC123&size=M&mauSac=trắng
        [HttpGet("tonkho")]
        public async Task<IActionResult> GetTonKhoTheoMaSPVaSize([FromQuery] string maSP, [FromQuery] string size, [FromQuery] string mauSac)
        {
            if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(size))
                return BadRequest("Mã sản phẩm và size không được để trống.");

            var chiTiet = await _context.ChiTietSanPham
                .FirstOrDefaultAsync(x => x.MaSanPham == maSP && x.Size == size && x.MauSac == mauSac );

            if (chiTiet == null)
                return NotFound("Không tìm thấy thông tin tồn kho cho sản phẩm, size và màu đã chọn.");

            return Ok(new
            {
                MaSP = chiTiet.MaSanPham,
                Size = chiTiet.Size,
                MauSac = chiTiet.MauSac,
                TonKho = chiTiet.TonKho
            });
        }


    }
}
