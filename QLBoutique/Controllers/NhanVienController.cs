using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public NhanVienController(BoutiqueDBContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<NhanVien>>> GetNhanViens()
        //{
        //    return await _context.NhanVien
        //                         .Include(nv => nv.QuyenHan)
        //                         .Include(nv => nv.ChucVu)
        //                         .ToListAsync();
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllNhanVien()
        {
            var list = await _context.NhanVien
                .Include(nv => nv.Quyen)
                .Include(nv => nv.ChucVu)
                .Select(nv => new
                {
                    nv.MaNV,
                    nv.HoTen,
                    nv.NgaySinh,
                    nv.GioiTinh,
                    nv.DiaChi,
                    nv.MaQuyen,
                    TenQuyen = nv.Quyen != null ? nv.Quyen.TenQuyen : null,
                    nv.MaCV,
                    TenChucVu = nv.ChucVu != null ? nv.ChucVu.TenCV : null,
                    nv.SDT,
                    nv.Email,
                    nv.NgayVaoLam
                })
                .ToListAsync();

            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> GetNhanVien(string id)
        {
            var nhanVien = await _context.NhanVien
                                         .Include(nv => nv.Quyen)
                                         .Include(nv => nv.ChucVu)
                                         .FirstOrDefaultAsync(nv => nv.MaNV == id);

            if (nhanVien == null)
                return NotFound();

            return nhanVien;
        }

        // ✅ Đã thêm mã hóa mật khẩu ở đây
        [HttpPost]
        public async Task<IActionResult> PostNhanVien([FromBody] NhanVien nhanvien)
        {
            if (nhanvien == null)
                return BadRequest("Dữ liệu nhân viên không hợp lệ.");

            if (string.IsNullOrEmpty(nhanvien.HoTen) || string.IsNullOrEmpty(nhanvien.DiaChi))
                return BadRequest("Họ tên và địa chỉ là bắt buộc.");

            // ✅ Gán Username và Password theo quyền + mã hóa mật khẩu
            string hoTenLower = nhanvien.HoTen.ToLower().Replace(" ", "");

            try
            {
                _context.NhanVien.Add(nhanvien);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNhanVien), new { id = nhanvien.MaNV }, nhanvien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi thêm nhân viên: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhanVien(string id, NhanVien nhanvien)
        {
            if (id != nhanvien.MaNV)
                return BadRequest();

            _context.Entry(nhanvien).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.NhanVien.Any(e => e.MaNV == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPut("Update/{maNV}")]
        public async Task<IActionResult> UpdateNhanVien(string maNV, [FromBody] NhanVien nhanVien)
        {
            if (maNV != nhanVien.MaNV)
                return BadRequest("Mã nhân viên không khớp.");

            var existingNV = await _context.NhanVien.FindAsync(maNV);
            if (existingNV == null)
                return NotFound("Không tìm thấy nhân viên.");

            existingNV.HoTen = nhanVien.HoTen;
            existingNV.NgaySinh = nhanVien.NgaySinh;
            existingNV.GioiTinh = nhanVien.GioiTinh;
            existingNV.DiaChi = nhanVien.DiaChi;
            existingNV.SDT = nhanVien.SDT;
            existingNV.Email = nhanVien.Email;
            existingNV.NgayVaoLam = nhanVien.NgayVaoLam;
            existingNV.MaCV = nhanVien.MaCV;
            existingNV.MaQuyen = nhanVien.MaQuyen;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cập nhật nhân viên thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi cập nhật nhân viên: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhanVien(string id)
        {
            var nhanvien = await _context.NhanVien.FindAsync(id);
            if (nhanvien == null)
                return NotFound();

            // Kiểm tra nhân viên có hóa đơn liên quan không
            bool hasRelatedHoaDon = await _context.HoaDon.AnyAsync(hd => hd.MaNV == id);

            if (hasRelatedHoaDon)
            {
                return BadRequest("Không thể xóa nhân viên này vì đang có hóa đơn liên quan.");
            }

            _context.NhanVien.Remove(nhanvien);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] QLBoutique.Model.LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Vui lòng nhập đầy đủ username và password");

            var user = await _context.NhanVien
                .AsNoTracking()
                .Where(nv => nv.Username == request.Username)
                .Select(nv => new
                {
                    nv.MaNV,
                    nv.Username,
                    nv.Password,
                    nv.MaQuyen,
                    nv.HoTen
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            string hashedInput = HashPassword(request.Password);
            if (user.Password != hashedInput)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            return Ok(new
            {
                user.MaNV,
                user.Username,
                user.MaQuyen,
                user.HoTen
            });
        }


        // ✅ Hàm hash mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanVien.Any(e => e.MaNV == id);
        }
    }
}
