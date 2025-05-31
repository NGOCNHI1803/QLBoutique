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
    public class QuyenHanController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public QuyenHanController(BoutiqueDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuyenHan>>> GetQuyenHan()
        {
            return await _context.QuyenHan.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuyenHan>> GetQuyenHan(string id)
        {
            var quyenHan = await _context.QuyenHan.FindAsync(id);
            if (quyenHan == null)
            {
                return NotFound();
            }
            return quyenHan;
        }

        [HttpPost]
        public async Task<ActionResult<QuyenHan>> PostQuyenHan(QuyenHan quyenHan)
        {
            _context.QuyenHan.Add(quyenHan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuyenHan), new { id = quyenHan.MaQuyen }, quyenHan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuyenHan(string id, QuyenHan quyenHan)
        {
            if (id != quyenHan.MaQuyen)
                return BadRequest();

            _context.Entry(quyenHan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.QuyenHan.Any(e => e.MaQuyen == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuyenHan(string id)
        {
            var quyenHan = await _context.QuyenHan.FindAsync(id);
            if (quyenHan == null)
                return NotFound();

            _context.QuyenHan.Remove(quyenHan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
