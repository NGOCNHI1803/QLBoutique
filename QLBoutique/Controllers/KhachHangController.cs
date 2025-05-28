using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLBoutique.ClothingDbContext;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;
        private readonly PasswordHasher<KhachHang> _passwordHasher;

        public KhachHangController(BoutiqueDBContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<KhachHang>();
        }

        [HttpPost("DangKy")]
        public async Task<IActionResult> DangKy([FromBody] KhachHang khachHang)
        {
            if (khachHang == null)
                return BadRequest("Dữ liệu khách hàng không hợp lệ.");

            // Kiểm tra bắt buộc: mật khẩu, email, số điện thoại
            if (string.IsNullOrEmpty(khachHang.MatKhau))
                return BadRequest("Mật khẩu không được để trống.");
            if (string.IsNullOrEmpty(khachHang.Email))
                return BadRequest("Email không được để trống.");
            if (string.IsNullOrEmpty(khachHang.SoDienThoai))
                return BadRequest("Số điện thoại không được để trống.");

            // Kiểm tra email đã tồn tại- TH có email tồn tại là đã có tài khoản đã tạo trên web
            var khachHangTheoEmail = await _context.KhachHang.FirstOrDefaultAsync(k => k.Email == khachHang.Email);
            if (khachHangTheoEmail != null)
                return BadRequest("Email đã tồn tại. Tài khoản đã có trong hệ thống!");

            // Kiểm tra số điện thoại đã tồn tại
            var khachHangTheoSoDT = await _context.KhachHang.FirstOrDefaultAsync(k => k.SoDienThoai == khachHang.SoDienThoai);

            if (khachHangTheoSoDT != null)
            {
                // Nếu số điện thoại tồn tại nhưng chưa có email, cập nhật thông tin - TH khách hàng đã đăng ký thông tin tại cửa hàng
                if (string.IsNullOrEmpty(khachHangTheoSoDT.Email))
                {
                    khachHangTheoSoDT.Email = khachHang.Email;
                    khachHangTheoSoDT.TenKH = khachHang.TenKH;
                    khachHangTheoSoDT.DiaChi = khachHang.DiaChi;
                    khachHangTheoSoDT.NgaySinh = khachHang.NgaySinh;
                    khachHangTheoSoDT.GhiChu = string.IsNullOrEmpty(khachHang.GhiChu) ? "Khách hàng mới" : khachHang.GhiChu;
                    khachHangTheoSoDT.MaLoaiKH = string.IsNullOrEmpty(khachHang.MaLoaiKH) ? "KHMOI" : khachHang.MaLoaiKH;
                    khachHangTheoSoDT.TrangThai = string.IsNullOrEmpty(khachHang.TrangThai) ? "Hoạt động" : khachHang.TrangThai;
                    khachHangTheoSoDT.NgayDangKy = DateTime.Now;

                    //// Hash mật khẩu nếu có
                    //if (!string.IsNullOrEmpty(khachHang.MatKhau))
                    //{
                    khachHangTheoSoDT.MatKhau = _passwordHasher.HashPassword(khachHangTheoSoDT, khachHang.MatKhau);
                    //}

                    _context.KhachHang.Update(khachHangTheoSoDT);
                    await _context.SaveChangesAsync();

                    return Ok("Cập nhật thông tin thành công.");
                }
                else
                {
                    // Số điện thoại đã tồn tại với email => tài khoản đã tạo trên web
                    return BadRequest("Tài khoản đã tồn tại. Xin hãy đăng nhập!");
                }


            }

            // Hàm sinh mã khách hàng KHxxxxx đảm bảo không trùng
            async Task<string> GenerateUniqueMaKHAsync()
            {
                string maKH;
                bool exists;
                var random = new Random();

                do
                {
                    maKH = "KH" + random.Next(0, 99999).ToString("D5");
                    exists = await _context.KhachHang.AnyAsync(k => k.MaKH == maKH);
                } while (exists);

                return maKH;
            }

            // Tạo khách hàng mới
            khachHang.MaKH = await GenerateUniqueMaKHAsync();
            khachHang.MatKhau = _passwordHasher.HashPassword(khachHang, khachHang.MatKhau);
            khachHang.GhiChu = string.IsNullOrEmpty(khachHang.GhiChu) ? "Khách hàng mới" : khachHang.GhiChu;
            khachHang.MaLoaiKH = string.IsNullOrEmpty(khachHang.MaLoaiKH) ? "KHT" : khachHang.MaLoaiKH;
            khachHang.TrangThai = string.IsNullOrEmpty(khachHang.TrangThai) ? "Hoạt động" : khachHang.TrangThai;
            khachHang.NgayDangKy = DateTime.Now;

            _context.KhachHang.Add(khachHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKhachHang), new { id = khachHang.MaKH }, khachHang);
        }


        // GET: api/KhachHang/DangNhap?email=abc@xyz.com&matKhau=123
        [HttpGet("DangNhap")]
        public async Task<IActionResult> DangNhap(string email, string matKhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
                return BadRequest("Email và mật khẩu là bắt buộc.");

            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(k => k.Email == email);

            if (khachHang == null)
            {
                return Unauthorized("Email không đúng.");
            }

            // Nếu khách hàng không hoạt động, không cho đăng nhập
            if (khachHang.TrangThai != "Hoạt động")
            {
                return Unauthorized($"Tài khoản hiện đang '{khachHang.TrangThai}' và không thể đăng nhập.");
            }

            var result = _passwordHasher.VerifyHashedPassword(khachHang, khachHang.MatKhau, matKhau);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Mật khẩu không đúng.");
            }

            return Ok(khachHang);
        }

        // GET: api/KhachHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHang>>> GetKhachHangs()
        {
            // Chỉ lấy khách hàng có trạng thái "Hoạt động"
            return await _context.KhachHang
                .Where(k => k.TrangThai == "Hoạt động")
                .ToListAsync();
        }

        // GET: api/KhachHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHang>> GetKhachHang(string id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);

            if (khachHang == null || khachHang.TrangThai == "Đã xóa")
            {
                return NotFound();
            }

            return khachHang;
        }
        private string GenerateMaKhachHang()
        {
            Random random = new Random();
            return "KH" + random.Next(0, 99999).ToString("D5"); // D5: định dạng thành chuỗi có 5 chữ số
        }
        // POST: api/KhachHang/ThemMoi
        [HttpPost("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromBody] KhachHang khachHang)
        {
            if (khachHang == null || string.IsNullOrEmpty(khachHang.MaKH))
                return BadRequest("Thông tin khách hàng không hợp lệ.");

            // Kiểm tra mã đã tồn tại
            var exists = await _context.KhachHang.AnyAsync(k => k.MaKH == khachHang.MaKH);
            if (exists)
                return BadRequest("Mã khách hàng đã tồn tại.");

            // Gán mặc định cho các giá trị không truyền từ FE
            khachHang.NgayDangKy = DateTime.Now;
            khachHang.MatKhau = "123"; // Có thể set mật khẩu mặc định hoặc random
            khachHang.GhiChu = string.IsNullOrEmpty(khachHang.GhiChu) ? "Khách hàng mới" : khachHang.GhiChu;
            khachHang.MaLoaiKH = string.IsNullOrEmpty(khachHang.MaLoaiKH) ? "KHT" : khachHang.MaLoaiKH;
            khachHang.TrangThai = "Hoạt động";

            // Nếu cần, có thể hash mật khẩu default
            khachHang.MatKhau = _passwordHasher.HashPassword(khachHang, khachHang.MatKhau);

            _context.KhachHang.Add(khachHang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKhachHang), new { id = khachHang.MaKH }, khachHang);
        }
        // GET: api/KhachHang/CheckSoDienThoai?soDienThoai=0123456789
        [HttpGet("CheckSoDienThoai")]
        public async Task<IActionResult> CheckSoDienThoai(string soDienThoai)
        {
            if (string.IsNullOrEmpty(soDienThoai))
                return BadRequest("Vui lòng cung cấp số điện thoại.");

            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

            if (khachHang != null)
            {
                return Ok(new
                {
                    TonTai = true,
                    DaCoEmail = !string.IsNullOrEmpty(khachHang.Email),
                    MaKH = khachHang.MaKH,
                    TenKH = khachHang.TenKH
                });
            }

            return Ok(new { TonTai = false });
        }



    }
}
