using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public ThongKeController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // api: lấy doanh thu hóa đơn theo ngày cụ  thể
        [HttpGet("doanhthungay")]
        public async Task<IActionResult> GetDoanhThuTheoNgaythang(DateTime ngay)
        {
            var doanhThu = await _context.HoaDon
                .Where(h => h.NgayLap.Date == ngay.Date)
                .SumAsync(h => h.ThanhTien);
            return Ok(doanhThu);
        }


        // api: lấy doanh thu hóa đơn theo từ ngày đến ngày
        [HttpGet("doanhthu/TuNgay-Ngay")]
        public async Task<IActionResult> GetDoanhThuTheoNgay([FromQuery] DateTime ngayStart, [FromQuery] DateTime ngayEnd)
        {
            var hoaDons = _context.HoaDon
                .Where(h => h.NgayLap.Date >= ngayStart.Date && h.NgayLap.Date <= ngayEnd.Date)
                .ToList();

            var tongTien = hoaDons.Sum(h => h.ThanhTien);

            return Ok(new ThongKe
            {
                Data = hoaDons,
                TongTien = tongTien
            });
        }

        //api: lấy doanh thu hóa đơn theo ngày
        [HttpGet("doanhthu/ngay")]
        public async Task<IActionResult> GetDoanhThuTheoNgay(DateTime ngay)
        {
            var doanhThu = await _context.HoaDon
                .Where(h => h.NgayLap.Date == ngay.Date)
                .SumAsync(h => h.ThanhTien);

            return Ok(new { Ngay = ngay.Date, DoanhThu = doanhThu });
        }

        // api: lấy doanh thu hóa đơn theo tuần
        [HttpGet("doanhthu/tuan")]
        public async Task<IActionResult> GetDoanhThuTheoTuan(DateTime ngayBatDau)
        {
            DateTime ngayKetThuc = ngayBatDau.AddDays(6);

            var doanhThu = await _context.HoaDon
                .Where(h => h.NgayLap.Date >= ngayBatDau.Date && h.NgayLap.Date <= ngayKetThuc.Date)
                .SumAsync(h => h.ThanhTien);

            return Ok(new
            {
                TuNgay = ngayBatDau.Date,
                DenNgay = ngayKetThuc.Date,
                DoanhThu = doanhThu
            });
        }

        // api: lấy doanh thu hóa đơn theo tháng
        [HttpGet("doanhthu/thang")]
        public async Task<IActionResult> GetDoanhThuTheoThang(int nam, int thang)
        {
            var doanhThu = await _context.HoaDon
                .Where(h => h.NgayLap.Year == nam && h.NgayLap.Month == thang)
                .SumAsync(h => h.ThanhTien);

            return Ok(new
            {
                Nam = nam,
                Thang = thang,
                DoanhThu = doanhThu
            });
        }


        // api: Lấy số lượng hàng tồn kho sản phẩm
        [HttpGet("hang-ton-kho")]
        public async Task<IActionResult> GetTongTonKhoTheoSanPham()
        {
            var tonKho = await _context.ChiTietSanPham
                .Include(ct => ct.SanPham)
                .GroupBy(ct => new
                {
                    ct.MaSanPham,
                    ct.SanPham.TenSanPham,
                    ct.GiaBan
                })
                .Select(g => new
                {
                    MaSP = g.Key.MaSanPham,
                    TenSP = g.Key.TenSanPham,
                    TongSoLuongTon = g.Sum(ct => ct.TonKho),
                    GiaBan = g.Key.GiaBan
                })
                .ToListAsync();

            return Ok(tonKho);
        }



    }
}
