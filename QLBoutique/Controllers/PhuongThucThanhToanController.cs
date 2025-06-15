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
    public class PhuongThucThanhToanController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;
        public PhuongThucThanhToanController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/PhuongThucThanhToan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhuongThucThanhToan>>> GetAll()
        {
            return await _context.PhuongThucThanhToan.ToListAsync();
        }

        //// POST: api/HoaDon
        //[HttpPost]
        //public async Task<ActionResult<HoaDon>> AddHoaDon([FromBody] HoaDon hoaDon)
        //{
        //    if (hoaDon == null)
        //        return BadRequest("Dữ liệu hóa đơn không hợp lệ.");

        //    try
        //    {
        //        _context.HoaDon.Add(hoaDon);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction(nameof(GetAll), new { id = hoaDon.MaHoaDon }, hoaDon);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return StatusCode(500, $"Lỗi lưu dữ liệu: {ex.Message}");
        //    }
        //}


    }
}