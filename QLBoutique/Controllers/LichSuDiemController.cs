using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuDiemController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public LichSuDiemController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // PUT: api/LichSuDiem/{maKH}
        [HttpPut("{maKH}")]
        public async Task<IActionResult> UpdateDiemByMaKH(string maKH, [FromBody] UpdateDiemRequest request)
        {
            if (string.IsNullOrEmpty(maKH))
                return BadRequest("Mã khách hàng không hợp lệ.");
            if (request == null || request.Diem < 0)
                return BadRequest("Điểm sử dụng không hợp lệ.");

            var lichSu = await _context.LichSuDiem
                .Where(x => x.MaKH == maKH)
                .OrderByDescending(x => x.Ngay)
                .FirstOrDefaultAsync();

            if (lichSu == null)
                return NotFound("Không tìm thấy lịch sử điểm cho khách hàng.");

            if (lichSu.Diem < request.Diem)
                return BadRequest("Số điểm sử dụng vượt quá số điểm hiện có.");

            // ✅ Trừ điểm
            lichSu.Diem -= request.Diem;
            lichSu.Ngay = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    MaKH = maKH,
                    DiemConLai = lichSu.Diem
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }






    }
}
