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

        // POST: api/HoaDon
        [HttpPost]
        public async Task<ActionResult<HoaDon>> AddHoaDon([FromBody] HoaDon hoaDon)
        {
            if (hoaDon == null)
                return BadRequest("Dữ liệu hóa đơn không hợp lệ.");

            try
            {
                _context.HoaDon.Add(hoaDon);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAll), new { id = hoaDon.MaHD }, hoaDon);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Lỗi lưu dữ liệu: {ex.Message}");
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
        [HttpPost("dathang")]
        public async Task<IActionResult> DatHang([FromBody] DatHangRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var diaChi = await _context.DiaChiKhachHang
                         .FirstOrDefaultAsync(dc => dc.MaDiaChi == request.MaDiaChi && dc.MaKH == request.MaKH);
                if (diaChi == null)
                {
                    return BadRequest("Địa chỉ giao hàng không hợp lệ hoặc không thuộc khách hàng này.");
                }
                decimal tongTien = request.SanPhams.Sum(sp => sp.GiaBan * sp.SoLuong);
                decimal tongGiamGia = request.SanPhams.Sum(sp => sp.GiamGia);
                // Lấy phí vận chuyển từ bảng DonViVanChuyen
                var donViVC = await _context.DonViVanChuyen.FirstOrDefaultAsync(x => x.MaDVVC == request.MaDVVC);
                if (donViVC == null)
                {
                    return BadRequest("Đơn vị vận chuyển không hợp lệ.");
                }

                decimal phiVanChuyen = donViVC.PhiVanChuyen;
                decimal thanhTien = tongTien - tongGiamGia + phiVanChuyen;

                var hoaDon = new HoaDon
                {
                    MaHD = "HD" + Guid.NewGuid().ToString("N").Substring(0, 10),
                    MaKH = request.MaKH,
                    MaNV = request.MaNV,
                    NgayLap = DateTime.Now,
                    MaKM = string.IsNullOrWhiteSpace(request.MaKM) ? null : request.MaKM,
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
                await _context.SaveChangesAsync();

                
                var gioHang = await _context.GioHang
                    .FirstOrDefaultAsync(g => g.MaKH == request.MaKH && g.TrangThai == 1);

                foreach (var sp in request.SanPhams)
                {
                    var bienThe = await _context.ChiTietSanPham.FindAsync(sp.MaBienThe);
                    if (bienThe == null || bienThe.TonKho < sp.SoLuong)
                    {
                        return BadRequest($"Không đủ hàng cho biến thể {sp.MaBienThe}");
                    }

                    bienThe.TonKho -= sp.SoLuong;

                    var chiTiet = new ChiTietHoaDon
                    {
                        MaChiTiet_HD = "CTHD" + Guid.NewGuid().ToString("N").Substring(0, 10),
                        MaHD = hoaDon.MaHD,
                        MaBienThe = sp.MaBienThe,
                        SoLuong = sp.SoLuong,
                        GiaBan = sp.GiaBan,
                        GiamGia = sp.GiamGia,
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

                return Ok(new { message = "Đặt hàng thành công", MaHD = hoaDon.MaHD });
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
                .Include(ct => ct.BienTheSanPham)
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
                .Include(ct => ct.BienTheSanPham)
                .Include(ct => ct.HoaDon)
                .ToListAsync();

            if (list == null || list.Count == 0)
                return NotFound($"Không tìm thấy chi tiết hóa đơn cho mã hóa đơn: {maHD}");

            return Ok(list);
        }





    }
}