using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
=======
using System.Security.Cryptography;
using System.Text;
>>>>>>> dbd1ab9 (Update backend)
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

        [HttpGet]
<<<<<<< HEAD
        public async Task<ActionResult<IEnumerable<NhanVien>>> GetNhanViens()
        {
            return await _context.NhanVien
                                 .Include(nv => nv.QuyenHan)
                                 .Include(nv => nv.ChucVu)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> GetNhanVien(string id)
        {
            var nhanVien = await _context.NhanVien
                                         .Include(nv => nv.QuyenHan)
                                         .Include(nv => nv.ChucVu)
                                         .FirstOrDefaultAsync(nv => nv.MaNV == id);

            if (nhanVien == null)
                return NotFound();

            return nhanVien;
        }

        [HttpPost]
        public async Task<ActionResult<NhanVien>> PostNhanVien(NhanVien nhanVien)
        {
            _context.NhanVien.Add(nhanVien);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNhanVien), new { id = nhanVien.MaNV }, nhanVien);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhanVien(string id, NhanVien nhanVien)
        {
            if (id != nhanVien.MaNV)
                return BadRequest();

            _context.Entry(nhanVien).State = EntityState.Modified;
=======
        public async Task<ActionResult<IEnumerable<Nhanvien>>> GetNhanViens()
        {
            return await _context.NhanVien.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Nhanvien>> GetNhanVien(string id)
        {
            var nhanvien = await _context.NhanVien.FindAsync(id);

            if (nhanvien == null)
                return NotFound();

            return nhanvien;
        }

        // ✅ Đã thêm mã hóa mật khẩu ở đây
        [HttpPost]
        public async Task<IActionResult> PostNhanVien([FromBody] Nhanvien nhanvien)
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
        public async Task<IActionResult> PutNhanVien(string id, Nhanvien nhanvien)
        {
            if (id != nhanvien.MaNV)
                return BadRequest();

            _context.Entry(nhanvien).State = EntityState.Modified;
>>>>>>> dbd1ab9 (Update backend)

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
<<<<<<< HEAD
                if (!_context.NhanVien.Any(e => e.MaNV == id))
=======
                if (!NhanVienExists(id))
>>>>>>> dbd1ab9 (Update backend)
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

<<<<<<< HEAD
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhanVien(string id)
        {
            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null)
                return NotFound();

            _context.NhanVien.Remove(nhanVien);
=======
        [HttpPut("Update/{maNV}")]
        public async Task<IActionResult> UpdateNhanVien(string maNV, [FromBody] Nhanvien nhanVien)
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
>>>>>>> dbd1ab9 (Update backend)
            await _context.SaveChangesAsync();

            return NoContent();
        }
<<<<<<< HEAD
=======


        // ✅ Login với password đã mã hóa
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] QLBoutique.Model.LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Vui lòng nhập đầy đủ username và password");

            var user = await _context.NhanVien.FirstOrDefaultAsync(nv => nv.Username == request.Username);

            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            string hashedInput = HashPassword(request.Password);

            if (user.Password != hashedInput)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            return Ok(new
            {
                user.MaNV,
                user.Username
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
>>>>>>> dbd1ab9 (Update backend)
    }
}
