using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLBoutique.ClothingDbContext;
using LabManagement.Model;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;
        public HoaDonController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/HoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetAll()
        {
            return await _context.HoaDon.ToListAsync();
        }
        [HttpPost]
        public async Task<IActionResult> AddHoaDon([FromBody] HoaDonRequest request)
        {
            if (request == null || request.ChiTietHoaDon == null || !request.ChiTietHoaDon.Any())
                return BadRequest("Dữ liệu hóa đơn không hợp lệ hoặc không có sản phẩm.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🔁 Sinh mã hóa đơn mới
                var lastHD = await _context.HoaDon.OrderByDescending(h => h.MaHoaDon).FirstOrDefaultAsync();
                string maMoi = "HD001";
                if (lastHD != null && !string.IsNullOrEmpty(lastHD.MaHoaDon) && lastHD.MaHoaDon.Length > 2)
                {
                    var soStr = lastHD.MaHoaDon.Substring(2);
                    if (int.TryParse(soStr, out int so))
                    {
                        maMoi = "HD" + (so + 1).ToString("D3");
                    }
                }

                var hoaDon = new HoaDon
                {
                    MaHoaDon = maMoi,
                    MaKH = request.MaKH,
                    MaNV = request.MaNV,
                    NgayLap = request.NgayLap,
                    TongTien = request.TongTien,
                    GiamGia = request.GiamGia,
                    ThanhTien = request.ThanhTien,
                    MaKM = request.MaKM,
                    MaTT = request.MaTT,
                    TrangThai = request.TrangThai,
                    GhiChu = request.GhiChu
                };

                _context.HoaDon.Add(hoaDon);
                await _context.SaveChangesAsync();

                // 🔁 Sinh mã chi tiết hóa đơn tăng dần
                var lastCT = await _context.ChiTietHoaDon.OrderByDescending(c => c.MaChiTietHD).FirstOrDefaultAsync();
                int ctSo = 1;
                if (lastCT != null && !string.IsNullOrEmpty(lastCT.MaChiTietHD) && lastCT.MaChiTietHD.Length > 2)
                {
                    string soStr = lastCT.MaChiTietHD.Substring(2);
                    if (int.TryParse(soStr, out int so))
                    {
                        ctSo = so + 1;
                    }
                }

                // 📦 Tạo chi tiết hóa đơn & cập nhật tồn kho
                foreach (var ct in request.ChiTietHoaDon)
                {
                    var bienThe = await _context.ChiTietSanPham.FindAsync(ct.MaBienThe);
                    if (bienThe == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Không tìm thấy biến thể sản phẩm với mã: {ct.MaBienThe}");
                    }

                    if (bienThe.TonKho < ct.SoLuong)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Sản phẩm {ct.MaBienThe} không đủ tồn kho.");
                    }

                    bienThe.TonKho -= ct.SoLuong;

                    decimal giaBan = bienThe.GiaBan ?? 0m;
                    decimal giamGia = hoaDon.GiamGia ?? 0m;
                    decimal thanhTien = (giaBan - giamGia) * ct.SoLuong;

                    var chiTietHD = new ChiTietHoaDon
                    {
                        MaChiTietHD = "CT" + ctSo.ToString("D3"),
                        MaHD = hoaDon.MaHoaDon,
                        MaBienThe = ct.MaBienThe,
                        SoLuong = ct.SoLuong,
                        GiaBan = bienThe.GiaBan,
                        GiaGiam = hoaDon.GiamGia,
                        ThanhTien = thanhTien
                    };

                    ctSo++; // tăng mã cho dòng tiếp theo
                    _context.ChiTietHoaDon.Add(chiTietHD);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Tạo hóa đơn và chi tiết thành công!", maHD = hoaDon.MaHoaDon });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Lỗi khi tạo hóa đơn: {ex.InnerException?.Message ?? ex.Message}");
            }
        }






    }
}