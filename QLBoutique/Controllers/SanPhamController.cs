using LabManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLBoutique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public SanPhamController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetAll()
        {
            return await _context.SanPham
                .Where(sp => !sp.isDeleted)
                .ToListAsync();
        }

        // GET: api/SanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetById(int id)
        {
            var sanPham = await _context.SanPham
                .FirstOrDefaultAsync(sp => sp.MaSanPham == id && !sp.isDeleted);

            if (sanPham == null)
            {
                return NotFound();
            }

            return sanPham;
        }

        // POST: api/SanPham
        [HttpPost]
        public async Task<ActionResult<SanPham>> Create(SanPham sanPham)
        {
            _context.SanPham.Add(sanPham);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = sanPham.MaSanPham }, sanPham);
        }

        // PUT: api/SanPham/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return BadRequest();
            }

            _context.Entry(sanPham).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanPhamExists(id))
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

        // DELETE: api/SanPham/5 (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sanPham = await _context.SanPham.FindAsync(id);
            if (sanPham == null || sanPham.isDeleted)
            {
                return NotFound();
            }

            sanPham.isDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPham.Any(sp => sp.MaSanPham == id && !sp.isDeleted);
        }

        // GET: api/SanPham/loai/5
        [HttpGet("loai/{maLoaiSP}")]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetSanPhamTheoLoaiSP(int maLoaiSP)
        {
            var danhSach = await _context.SanPham
                                         .Where(sp => sp.MaLoai == maLoaiSP && !sp.isDeleted)
                                         .ToListAsync();

            if (danhSach == null || !danhSach.Any())
            {
                return NotFound("Không tìm thấy sản phẩm thuộc loại này.");
            }

            return Ok(danhSach);
        }


    }
}
