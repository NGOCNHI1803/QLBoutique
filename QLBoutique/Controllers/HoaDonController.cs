using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLBoutique.ClothingDbContext;
using LabManagement.Model;
using QLBoutique.Model.DTO;
using System;

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
        // GET: api/HoaDon/{maHoaDon}
        [HttpGet("{maHoaDon}")]
        public async Task<ActionResult<HoaDon>> GetByMaHoaDon(string maHoaDon)
        {
            var hoaDon = await _context.HoaDon
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.MaHoaDon == maHoaDon);

            if (hoaDon == null) return NotFound();

            var dto = new HoaDon
            {
                MaHoaDon = hoaDon.MaHoaDon,
                MaKH = hoaDon.MaKH,
                MaNV = hoaDon.MaNV,
                NgayLap = hoaDon.NgayLap,
                TongTien = hoaDon.TongTien,
                GiamGia = hoaDon.GiamGia,
                ThanhTien = hoaDon.ThanhTien,
                MaKM = hoaDon.MaKM,
                TrangThai = hoaDon.TrangThai,
                GhiChu = hoaDon.GhiChu,
                MaDiaChi = hoaDon.MaDiaChi,
                MaTT = hoaDon.MaTT,
                MaDVVC = hoaDon.MaDVVC,
                TrangThai_VanChuyen = hoaDon.TrangThai_VanChuyen
            };

            return dto;
        }


        /* Tạo hóa đơn cho winform*/
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
                    GhiChu = request.GhiChu,
                    TrangThai_VanChuyen = request.TrangThai_VanChuyen
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
        // --- Lấy địa chỉ khách hàng theo MaKH
        [HttpGet("diachikhachhang/{maKH}")]
        public async Task<ActionResult<IEnumerable<DiaChiKhachHang>>> GetDiaChiKhachHang(string maKH)
        {
            var diaChiList = await _context.DiaChiKhachHang
                .Where(dc => dc.MaKH == maKH)
                .ToListAsync();

            if (diaChiList == null || diaChiList.Count == 0)
                return NotFound($"Không tìm thấy địa chỉ cho khách hàng {maKH}");

            return Ok(diaChiList);
        }
        [HttpPost("diachikhachhang")]
        public async Task<IActionResult> AddDiaChiKhachHang([FromBody] DiaChiKhachHangDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.MaKH))
                return BadRequest("Thông tin địa chỉ không hợp lệ.");

            try
            {
                var diaChi = new DiaChiKhachHang
                {
                    MaDiaChi = "DC" + Guid.NewGuid().ToString("N").Substring(0, 8),
                    MaKH = dto.MaKH,
                    HoTenNguoiNhan = dto.HoTenNguoiNhan,
                    SDTNguoiNhan = dto.SdtNguoiNhan,
                    DiaChiCuThe = dto.DiaChiCuThe,
                    TinhTP = dto.TinhTP,
                    HuyenQuan = dto.HuyenQuan,
                    XaPhuong = dto.XaPhuong,
                    MacDinh = dto.MacDinh,
                    GhiChu = dto.GhiChu
                };

                _context.DiaChiKhachHang.Add(diaChi);
                await _context.SaveChangesAsync();

                return Ok(diaChi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi thêm địa chỉ: {ex.Message}");
            }
        }


        [HttpPut("diachikhachhang/{maDiaChi}")]
        public async Task<IActionResult> UpdateDiaChiKhachHang(string maDiaChi, [FromBody] DiaChiKhachHang updatedDiaChi)
        {
            var existing = await _context.DiaChiKhachHang.FirstOrDefaultAsync(d => d.MaDiaChi == maDiaChi);

            if (existing == null)
                return NotFound("Không tìm thấy địa chỉ cần cập nhật.");

            if (existing.MaKH != updatedDiaChi.MaKH)
                return BadRequest("Không thể thay đổi mã khách hàng.");

            try
            {
                existing.HoTenNguoiNhan = updatedDiaChi.HoTenNguoiNhan;
                existing.SDTNguoiNhan = updatedDiaChi.SDTNguoiNhan;
                existing.DiaChiCuThe = updatedDiaChi.DiaChiCuThe;
                existing.HuyenQuan = updatedDiaChi.HuyenQuan;
                existing.TinhTP = updatedDiaChi.TinhTP;
                existing.XaPhuong = updatedDiaChi.XaPhuong;
                existing.GhiChu = updatedDiaChi.GhiChu;

                await _context.SaveChangesAsync();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi cập nhật địa chỉ: {ex.Message}");
            }
        }
        [HttpDelete("diachikhachhang/{maDiaChi}")]
        public async Task<IActionResult> DeleteDiaChiKhachHang(string maDiaChi)
        {
            var diaChi = await _context.DiaChiKhachHang.FirstOrDefaultAsync(d => d.MaDiaChi == maDiaChi);

            if (diaChi == null)
                return NotFound("Địa chỉ không tồn tại.");

            try
            {
                _context.DiaChiKhachHang.Remove(diaChi);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Xóa địa chỉ thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa địa chỉ: {ex.Message}");
            }
        }


        [HttpGet("donvivanchuyen")]
        public async Task<ActionResult<IEnumerable<DonViVanChuyen>>> GetDonViVanChuyen()
        {
            var dsDonVi = await _context.DonViVanChuyen.ToListAsync();
            return Ok(dsDonVi);
        }

        // --- Lấy danh sách khuyến mãi
        [HttpGet("khuyenmai")]
        public async Task<ActionResult<IEnumerable<KhuyenMai>>> GetKhuyenMai()
        {
            var danhSach = await _context.KhuyenMai
   .Select(k => new {
       k.MaKM,
       k.TenKM,
       k.MoTa,
       k.MaLoaiKM,
       PhanTramGiam = k.PhanTramGiam ?? 0,
       TrangThai = k.TrangThai ?? 0,
       SoLuongApDung = k.SoLuongApDung ?? 0,
       SoLuongDaApDung = k.SoLuongDaApDung ?? 0,
       k.NgayBatDau,
       k.NgayKetThuc
   })
   .ToListAsync();

            return Ok(danhSach);
        }

        // --- Lấy danh sách phương thức thanh toán
        [HttpGet("phuongthucthanhtoan")]
        public async Task<ActionResult<IEnumerable<PhuongThucThanhToan>>> GetPhuongThucThanhToan()
        {
            var ptThanhToanList = await _context.PhuongThucThanhToan.ToListAsync();
            return Ok(ptThanhToanList);
        }
        // Sinh mã hóa đơn mới: HD001, HD002,...
        private async Task<string> GenerateMaHoaDonAsync()
        {
            var lastHD = await _context.HoaDon
                .OrderByDescending(h => h.MaHoaDon)
                .Select(h => h.MaHoaDon)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastHD) && lastHD.StartsWith("HD"))
            {
                var numberPart = lastHD.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"HD{nextNumber:D3}"; // VD: HD001
        }

        // Sinh danh sách mã ChiTietHoaDon: CTHD001, CTHD002,...
        private async Task<List<string>> GenerateDanhSachMaChiTietHDAsync(int soLuong)
        {
            var lastCTHD = await _context.ChiTietHoaDon
                .OrderByDescending(ct => ct.MaChiTietHD)
                .Select(ct => ct.MaChiTietHD)
                .FirstOrDefaultAsync();

            int lastNumber = 0;
            if (!string.IsNullOrEmpty(lastCTHD) && lastCTHD.StartsWith("CTHD"))
            {
                var numberPart = lastCTHD.Substring(4);
                int.TryParse(numberPart, out lastNumber);
            }

            var danhSach = new List<string>();
            for (int i = 1; i <= soLuong; i++)
            {
                danhSach.Add($"CTHD{(lastNumber + i):D3}");
            }

            return danhSach;
        }
        [HttpPost("dathang")]
        public async Task<IActionResult> DatHang([FromBody] DatHangRequest request)
        {
            if (request == null || request.SanPhams == null || !request.SanPhams.Any())
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var diaChi = await _context.DiaChiKhachHang
                    .FirstOrDefaultAsync(dc => dc.MaDiaChi == request.MaDiaChi && dc.MaKH == request.MaKH);

                if (diaChi == null)
                    return BadRequest("Địa chỉ giao hàng không hợp lệ hoặc không thuộc khách hàng này.");

                var donViVC = await _context.DonViVanChuyen
                    .FirstOrDefaultAsync(x => x.MaDVVC == request.MaDVVC);

                if (donViVC == null)
                    return BadRequest("Đơn vị vận chuyển không hợp lệ.");

                decimal tongTien = request.SanPhams.Sum(sp => sp.GiaBan * sp.SoLuong);
                decimal tongGiamGia = request.SanPhams.Sum(sp => sp.GiamGia);
                decimal phiVanChuyen = donViVC.PhiVanChuyen;
                decimal thanhTien = tongTien - tongGiamGia + phiVanChuyen;

                var hoaDon = new HoaDon
                {
                    MaHoaDon = await GenerateMaHoaDonAsync(),
                    MaKH = request.MaKH,
                    MaNV = request.MaNV,
                    NgayLap = DateTime.Now,
                    MaKM = request.MaKM,
                    MaDiaChi = request.MaDiaChi,
                    GhiChu = request.GhiChu,
                    TongTien = tongTien,
                    GiamGia = tongGiamGia,
                    ThanhTien = thanhTien,
                    MaTT = request.MaTT,
                    MaDVVC = request.MaDVVC,
                    TrangThai = 1
                };

                _context.HoaDon.Add(hoaDon);
                await _context.SaveChangesAsync(); // Lưu Hóa đơn trước để lấy MaHD

                var gioHang = await _context.GioHang
                    .FirstOrDefaultAsync(g => g.MaKH == request.MaKH && g.TrangThai == 1);

                var danhSachMaCTHD = await GenerateDanhSachMaChiTietHDAsync(request.SanPhams.Count);
                int index = 0;

                foreach (var sp in request.SanPhams)
                {
                    var bienThe = await _context.ChiTietSanPham.FindAsync(sp.MaBienThe);
                    if (bienThe == null || bienThe.TonKho < sp.SoLuong)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Không đủ hàng cho biến thể {sp.MaBienThe}");
                    }

                    bienThe.TonKho -= sp.SoLuong;

                    var chiTiet = new ChiTietHoaDon
                    {
                        MaChiTietHD = danhSachMaCTHD[index++],
                        MaHD = hoaDon.MaHoaDon,
                        MaBienThe = sp.MaBienThe,
                        SoLuong = sp.SoLuong,
                        GiaBan = sp.GiaBan,
                        GiaGiam = sp.GiamGia,
                        ThanhTien = (sp.GiaBan * sp.SoLuong) - sp.GiamGia
                    };

                    _context.ChiTietHoaDon.Add(chiTiet);

                    if (gioHang != null)
                    {
                        var gioHangItem = await _context.ChiTietGioHang
                            .FirstOrDefaultAsync(x => x.MaGioHang == gioHang.MaGioHang && x.MaBienThe == sp.MaBienThe);

                        if (gioHangItem != null)
                        {
                            _context.ChiTietGioHang.Remove(gioHangItem);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Đặt hàng thành công", maHoaDon = hoaDon.MaHoaDon });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Lỗi xử lý đơn hàng: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        // GET: api/HoaDon/chitiethoadon
        [HttpGet("chitiethoadon")]
        public async Task<ActionResult<IEnumerable<ChiTietHoaDon>>> GetAllChiTietHoaDon()
        {
            var list = await _context.ChiTietHoaDon
                .Include(ct => ct.ChiTietSanPham)
                .Include(ct => ct.HoaDon)
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/HoaDon/chitiethoadon/{maHD}
        [HttpGet("chitiethoadon/{maHD}")]
        public async Task<ActionResult<IEnumerable<ChiTietHoaDon>>> GetChiTietHoaDonTheoMaHD(string maHD)
        {
            var list = await _context.ChiTietHoaDon
                .Where(ct => ct.MaHD == maHD)
                .Include(ct => ct.ChiTietSanPham)
                .Include(ct => ct.HoaDon)
                .ToListAsync();

            if (list == null || list.Count == 0)
                return NotFound($"Không tìm thấy chi tiết hóa đơn cho mã hóa đơn: {maHD}");

            return Ok(list);
        }









    }
}