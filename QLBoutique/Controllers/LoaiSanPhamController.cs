﻿using Microsoft.AspNetCore.Mvc;
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
    public class LoaiSanPhamController : ControllerBase
    {
        private readonly BoutiqueDBContext _context;

        public LoaiSanPhamController(BoutiqueDBContext context)
        {
            _context = context;
        }

        // GET: api/LoaiSanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiSanPham>>> GetLoaiSanPhams()
        {
            return await _context.LoaiSanPham.ToListAsync();
        }

        // GET: api/LoaiSanPham/ma
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiSanPham>> GetLoaiSanPham(string id)
        {
            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);

            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return loaiSanPham;
        }

        // POST: api/LoaiSanPham
        [HttpPost]
        public async Task<ActionResult<LoaiSanPham>> PostLoaiSanPham(LoaiSanPham loaiSanPham)
        {
            _context.LoaiSanPham.Add(loaiSanPham);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoaiSanPham), new { id = loaiSanPham.MaLoai }, loaiSanPham);
        }

        // PUT: api/LoaiSanPham/ma
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiSanPham(string id, LoaiSanPham loaiSanPham)
        {
            if (id != loaiSanPham.MaLoai)
            {
                return BadRequest();
            }

            _context.Entry(loaiSanPham).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoaiSanPhamExists(id))
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

        // DELETE: api/LoaiSanPham/ma
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiSanPham(string id)
        {
            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            _context.LoaiSanPham.Remove(loaiSanPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoaiSanPhamExists(string id)
        {
            return _context.LoaiSanPham.Any(e => e.MaLoai == id);
        }
    }
}
