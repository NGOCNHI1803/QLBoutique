using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QLBoutique.Model;
using QLBoutique.ClothingDbContext;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography;

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

        // Đăng ký khách hàng
        [HttpPost("DangKy")]
        public async Task<IActionResult> DangKy([FromBody] KhachHang khachHang)
        {
            if (khachHang == null)
                return BadRequest("Dữ liệu khách hàng không hợp lệ.");

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
                    khachHangTheoSoDT.NgaySinh = khachHang.NgaySinh;
                    khachHangTheoSoDT.GhiChu = string.IsNullOrEmpty(khachHang.GhiChu) ? "Khách hàng mới" : khachHang.GhiChu;
                    khachHangTheoSoDT.MaLoaiKH = string.IsNullOrEmpty(khachHang.MaLoaiKH) ? "KHM" : khachHang.MaLoaiKH;
                    khachHangTheoSoDT.TrangThai = string.IsNullOrEmpty(khachHang.TrangThai) ? "Hoạt động" : khachHang.TrangThai;
                    khachHangTheoSoDT.NgayDangKy = DateTime.Now;

                    khachHangTheoSoDT.MatKhau = _passwordHasher.HashPassword(khachHangTheoSoDT, khachHang.MatKhau);
                    _context.KhachHang.Update(khachHangTheoSoDT);
                    await _context.SaveChangesAsync();

                    return Ok("Cập nhật thông tin thành công.");
                }
                else
                {
                    return BadRequest("Tài khoản đã tồn tại. Xin hãy đăng nhập!");
                }
            }

            // Tạo mã khách hàng KHxxxxx không trùng
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

        // Đăng nhập
        [HttpGet("DangNhap")]
        public async Task<IActionResult> DangNhap(string email, string matKhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
                return BadRequest("Email và mật khẩu là bắt buộc.");

            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(k => k.Email == email);
            if (khachHang == null)
                return Unauthorized("Email không đúng.");

            if (khachHang.TrangThai != "Hoạt động")
                return Unauthorized($"Tài khoản hiện đang '{khachHang.TrangThai}' và không thể đăng nhập.");

            PasswordVerificationResult result;

            try
            {
                // Kiểm tra mật khẩu hash
                result = _passwordHasher.VerifyHashedPassword(khachHang, khachHang.MatKhau, matKhau);
            }
            catch (FormatException)
            {
                // Trường hợp mật khẩu lưu plaintext, kiểm tra và hash lại
                if (khachHang.MatKhau == matKhau)
                {
                    khachHang.MatKhau = _passwordHasher.HashPassword(khachHang, matKhau);
                    await _context.SaveChangesAsync();
                    result = PasswordVerificationResult.Success;
                }
                else
                {
                    return Unauthorized("Mật khẩu không đúng.");
                }
            }

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Mật khẩu không đúng.");

            return Ok(khachHang);
        }

        // Lấy danh sách khách hàng trạng thái Hoạt động
        [HttpGet]
        public async Task<IActionResult> GetAllKhachHang()
        {
            var khachHangs = await _context.KhachHang.Where(k => k.TrangThai == "Hoạt động").ToListAsync();
            return Ok(khachHangs);
        }

        // Lấy thông tin khách hàng theo mã
        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHang>> GetKhachHang(string id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);

            if (khachHang == null || khachHang.TrangThai == "Đã xóa")
                return NotFound();

            return khachHang;
        }

        // Kiểm tra số điện thoại đã tồn tại
        [HttpGet("CheckSoDienThoai")]
        public async Task<IActionResult> CheckSoDienThoai(string soDienThoai)
        {
            if (string.IsNullOrEmpty(soDienThoai))
                return BadRequest("Vui lòng cung cấp số điện thoại.");

            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

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
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] KhachHang model)
        {
            if (model == null ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.ResetPasswordToken) ||
                string.IsNullOrEmpty(model.MatKhau)) // MatKhau là mật khẩu mới
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(k => k.Email == model.Email);
            if (khachHang == null)
                return BadRequest("Email không tồn tại.");

            if (khachHang.ResetPasswordToken != model.ResetPasswordToken || khachHang.ResetPasswordExpiry < DateTime.UtcNow)
                return BadRequest("Token đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");

            // Hash mật khẩu mới
            khachHang.MatKhau = _passwordHasher.HashPassword(khachHang, model.MatKhau);

            // Xóa token và thời gian hết hạn
            khachHang.ResetPasswordToken = null;
            khachHang.ResetPasswordExpiry = null;

            _context.KhachHang.Update(khachHang);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công." });
        }
        // Quên mật khẩu - gửi email reset password
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Vui lòng nhập email.");

            var khachHang = await _context.KhachHang.FirstOrDefaultAsync(k => k.Email == email);
            if (khachHang == null)
                return BadRequest("Email không tồn tại.");

            // Tạo token mạnh
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);
            var expiry = DateTime.UtcNow.AddHours(1);

            khachHang.ResetPasswordToken = token;
            khachHang.ResetPasswordExpiry = expiry;
            await _context.SaveChangesAsync();

            // Tạo link reset password (encode để tránh lỗi url)
            var resetLink = $"http://localhost:3000/clothing-shop-app/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            // Tạo email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("QLBoutique", "no-reply@yourdomain.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Đặt lại mật khẩu FASIC.VN";

            message.Body = new TextPart("plain")
            {
                Text = $"Xin chào,\n\nBạn đã yêu cầu đặt lại mật khẩu. Vui lòng truy cập link dưới đây để đặt lại mật khẩu:\n\n{resetLink}\n\nLink chỉ có hiệu lực trong 1 giờ.\n\nNếu bạn không yêu cầu, hãy bỏ qua email này."
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("phanthithanhnga1303@gmail.com", "chgijvbkajpjdilh"); 
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ở đây nếu có logger
                return StatusCode(500, "Gửi email thất bại: " + ex.Message);
            }

            return Ok("Đã gửi email đặt lại mật khẩu. Vui lòng kiểm tra hộp thư!");
        }
    
        [HttpPost("create")]
        public async Task<IActionResult> CreateKhachHang([FromBody] KhachHang kh)
        {
            if (kh == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            // Hash mật khẩu trước
            var hashedPassword = _passwordHasher.HashPassword(kh, kh.MatKhau);

            var newKh = new KhachHang
            {
                MaKH = kh.MaKH,
                TenKH = kh.TenKH,
                DiaChi = kh.DiaChi,
                SoDienThoai = kh.SoDienThoai,
                MaLoaiKH = kh.MaLoaiKH ?? "KHT",
                TaiKhoan = kh.TaiKhoan,
                MatKhau = hashedPassword,
                GhiChu = kh.GhiChu ?? "Khách hàng mới",
                NgaySinh = kh.NgaySinh ?? DateTime.MinValue,
                NgayDangKy = DateTime.Now,
                TrangThai = "Hoạt động",

                // Các thuộc tính khác giữ mặc định hoặc null
                // Email = null,
                // NgaySinh = null,
                // IsEmailConfirmed = false,
                // EmailConfirmationToken = null,
                // EmailTokenExpiry = null,
            };

            _context.KhachHang.Add(newKh);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm khách hàng thành công", newKh.MaKH });
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
        //// GET: api/KhachHang/CheckSoDienThoai?soDienThoai=0123456789
        //[HttpGet("CheckSoDienThoai")]
        //public async Task<IActionResult> CheckSoDienThoai(string soDienThoai)
        //{
        //    if (string.IsNullOrEmpty(soDienThoai))
        //        return BadRequest("Vui lòng cung cấp số điện thoại.");

        //    var khachHang = await _context.KhachHang
        //        .FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

        //    if (khachHang != null)
        //    {
        //        return Ok(new
        //        {
        //            TonTai = true,
        //            DaCoEmail = !string.IsNullOrEmpty(khachHang.Email),
        //            MaKH = khachHang.MaKH,
        //            TenKH = khachHang.TenKH
        //        });
        //    }

        //    return Ok(new { TonTai = false });
        //}

        // Tìm kiếm theo số điện thoại khách hàng
        // GET: api/KhachHang/search?SDT=xxx
        [HttpGet("search")]
        public async Task<IActionResult> SearchByPhoneNumberCus([FromQuery] string SDT)
        {
            if (string.IsNullOrWhiteSpace(SDT))
                return BadRequest("Bạn phải cung cấp số điện thoại.");

            var list = await _context.KhachHang
                .AsNoTracking()
                .Where(k => k.SoDienThoai == SDT)
                .Select(k => new
                {
                    k.MaKH,
                    k.Email,
                    k.MatKhau,
                    k.TenKH,
                    k.DiaChi,
                    k.SoDienThoai,
                    k.NgaySinh,
                    k.GhiChu,
                    k.MaLoaiKH,
                    k.NgayDangKy
                })
                .ToListAsync();

            if (list == null || list.Count == 0)
                return NotFound("Không tìm thấy khách hàng nào với số điện thoại đã cho.");

            return Ok(list);
        }

        // GET: api/KhachHang/getPoint
        [HttpGet("Search_point")]
        public async Task<IActionResult> TimKiemKHVaDiem([FromQuery] string tenKH, [FromQuery] string sdt)
        {
            if (string.IsNullOrWhiteSpace(tenKH) || string.IsNullOrWhiteSpace(sdt))
                return BadRequest("Tên và SĐT không được để trống.");

            var khachHang = await _context.KhachHang.AsNoTracking()
                .FirstOrDefaultAsync(kh => kh.TenKH == tenKH && kh.SoDienThoai == sdt);

            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            var tongDiem = await _context.LichSuDiem.AsNoTracking()
                .Where(d => d.MaKH == khachHang.MaKH)
                .SumAsync(d => (int?)d.Diem) ?? 0;

            return Ok(new
            {
                MaKH = khachHang.MaKH,
                TenKH = khachHang.TenKH,
                SoDienThoai = khachHang.SoDienThoai,
                DiemTichLuy = tongDiem
            });
        }

        // GET: api/KhachHang/getID
        [HttpGet("GetMaKH")]
        public async Task<IActionResult> GetMaKH([FromQuery] string tenKH, [FromQuery] string sdt)
        {
            if (string.IsNullOrWhiteSpace(tenKH) || string.IsNullOrWhiteSpace(sdt))
                return BadRequest("Tên và số điện thoại không được để trống.");

            tenKH = tenKH.Trim().ToLower();
            sdt = sdt.Trim();

            var khachHang = await _context.KhachHang
                .AsNoTracking()
                .FirstOrDefaultAsync(k =>
                    k.TenKH.ToLower().Trim() == tenKH &&
                    k.SoDienThoai.Trim() == sdt);

            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            return Ok(new { MaKH = khachHang.MaKH });
        }


        // DELETE: /KhachHang/Delete/{maKH}
        [HttpDelete("Delete/{maKH}")]
        public async Task<IActionResult> DeleteCustomer(string maKH)
        {
            var customer = await _context.KhachHang.FindAsync(maKH);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng cần xóa." });
            }

            _context.KhachHang.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa khách hàng thành công." });
        }

    }
}
