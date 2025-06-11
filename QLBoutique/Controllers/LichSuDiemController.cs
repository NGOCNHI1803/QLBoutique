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

        [HttpPut("{maKH}")]
        public async Task<IActionResult> UpdateDiemByMaKH(string maKH, [FromBody] LichSuDiem updateModel)
        {
            if (string.IsNullOrEmpty(maKH))
                return BadRequest("Mã khách hàng không hợp lệ.");

            // Tìm bản ghi mới nhất của khách hàng đó
            var lichSu = await _context.LichSuDiem
                .Where(x => x.MaKH == maKH)
                .OrderByDescending(x => x.Ngay)
                .FirstOrDefaultAsync();

            if (lichSu == null)
                return NotFound("Không tìm thấy lịch sử điểm cho khách hàng.");

            // Cập nhật điểm
            lichSu.Diem = updateModel.Diem;

            await _context.SaveChangesAsync();

            return Ok(lichSu);
        }




    }
}
