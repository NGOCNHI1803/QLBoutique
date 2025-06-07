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
    public class NhaCungCapController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public NhaCungCapController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/NhaCungCap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaCungCap>>> GetNhaCungCaps()
        {
            return await _context.NhaCungCap.ToListAsync();
        }

        // GET: api/NhaCungCap/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhaCungCap>> GetNhaCungCap(string id)
        {
            var nhaCungCap = await _context.NhaCungCap.FindAsync(id);

            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return nhaCungCap;
        }

        // POST: api/NhaCungCap
        [HttpPost]
        public async Task<ActionResult<NhaCungCap>> PostNhaCungCap(NhaCungCap nhaCungCap)
        {
            _context.NhaCungCap.Add(nhaCungCap);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNhaCungCap), new { id = nhaCungCap.MaNCC }, nhaCungCap);
        }

        // PUT: api/NhaCungCap/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhaCungCap(string id, NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNCC)
            {
                return BadRequest();
            }

            _context.Entry(nhaCungCap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaCungCapExists(id))
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

        // PUT: api/NhaCungCap/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> PutUpdateNhaCungCap(string id, NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNCC)
            {
                return BadRequest("Mã nhà cung cấp không khớp.");
            }

            var existingSupplier = await _context.NhaCungCap.FindAsync(id);
            if (existingSupplier == null)
            {
                return NotFound("Không tìm thấy nhà cung cấp.");
            }

            // Cập nhật các trường
            existingSupplier.TenNCC = nhaCungCap.TenNCC;
            existingSupplier.DiaChi = nhaCungCap.DiaChi;
            existingSupplier.SDT = nhaCungCap.SDT;
            existingSupplier.Email = nhaCungCap.Email;
            existingSupplier.TrangThai = nhaCungCap.TrangThai;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingSupplier);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Lỗi cập nhật dữ liệu: {ex.Message}");
            }
        }

        // DELETE: api/NhaCungCap/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhaCungCap(string id)
        {
            var nhaCungCap = await _context.NhaCungCap.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            _context.NhaCungCap.Remove(nhaCungCap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhaCungCapExists(string id)
        {
            return _context.NhaCungCap.Any(e => e.MaNCC == id);
        }
    }
}
