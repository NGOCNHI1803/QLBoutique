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
    public class ChucVuController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public ChucVuController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/ChucVu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChucVu>>> GetChucVus()
        {
            return await _context.ChucVu.ToListAsync();
        }

        // GET: api/ChucVu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ChucVu>> GetChucVu(int id)
        {
            var chucVu = await _context.ChucVu.FindAsync(id);

            if (chucVu == null)
            {
                return NotFound();
            }

            return chucVu;
        }

        // POST: api/ChucVu
        [HttpPost]
        public async Task<ActionResult<ChucVu>> PostChucVu(ChucVu chucVu)
        {
            _context.ChucVu.Add(chucVu);
            await _context.SaveChangesAsync();

            // Đảm bảo id được trả về đúng
            return CreatedAtAction(nameof(GetChucVu), new { id = chucVu.MaChucVu }, chucVu);
        }

        // PUT: api/ChucVu/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChucVu(int id, ChucVu chucVu)
        {
            if (id != chucVu.MaChucVu)
            {
                return BadRequest();
            }

            _context.Entry(chucVu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChucVuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ChucVu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChucVu(int id)  // Thay đổi kiểu id từ string thành int
        {
            var chucVu = await _context.ChucVu.FindAsync(id);
            if (chucVu == null)
            {
                return NotFound();
            }

            _context.ChucVu.Remove(chucVu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChucVuExists(int id)
        {
            return _context.ChucVu.Any(e => e.MaChucVu == id);
        }
    }
}
