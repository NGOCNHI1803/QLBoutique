using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using QLBoutique.Model.DTO;
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
        public async Task<ActionResult<IEnumerable<LoaiSanPhamDTO>>> GetLoaiSanPhams()
        {
            var allLoai = await _context.LoaiSanPham.ToListAsync();

            var loaiSanPhamDict = allLoai.ToDictionary(
                x => x.MaLoai!,
                x => new LoaiSanPhamDTO
                {
                    MaLoai = x.MaLoai,
                    TenLoai = x.TenLoai,
                    XuatSu = x.XuatSu,
                    Children = new List<LoaiSanPhamDTO>()
                });

            List<LoaiSanPhamDTO> danhSachPhanCap = new List<LoaiSanPhamDTO>();

            foreach (var loai in allLoai)
            {
                if (!string.IsNullOrEmpty(loai.ParentId) && loaiSanPhamDict.ContainsKey(loai.ParentId))
                {
                    loaiSanPhamDict[loai.ParentId].Children!.Add(loaiSanPhamDict[loai.MaLoai!]);
                }
                else
                {
                    // Là loại gốc không có cha
                    danhSachPhanCap.Add(loaiSanPhamDict[loai.MaLoai!]);
                }
            }

            return Ok(danhSachPhanCap);
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

        [HttpPost]
        public async Task<ActionResult<LoaiSanPham>> PostLoaiSanPham(LoaiSanPham loaiSanPham)
        {
            if (!string.IsNullOrEmpty(loaiSanPham.ParentId))
            {
                var parentExists = await _context.LoaiSanPham.AnyAsync(x => x.MaLoai == loaiSanPham.ParentId);
                if (!parentExists)
                {
                    return BadRequest("ParentId không tồn tại.");
                }
            }

            _context.LoaiSanPham.Add(loaiSanPham);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoaiSanPham), new { id = loaiSanPham.MaLoai }, loaiSanPham);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiSanPham(string id, LoaiSanPham loaiSanPham)
        {
            if (id != loaiSanPham.MaLoai)
            {
                return BadRequest("ID không khớp.");
            }

            if (!string.IsNullOrEmpty(loaiSanPham.ParentId))
            {
                if (loaiSanPham.ParentId == id)
                {
                    return BadRequest("ParentId không thể trùng với chính nó.");
                }

                var parentExists = await _context.LoaiSanPham.AnyAsync(x => x.MaLoai == loaiSanPham.ParentId);
                if (!parentExists)
                {
                    return BadRequest("ParentId không tồn tại.");
                }
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiSanPham(string id)
        {
            var loaiSanPham = await _context.LoaiSanPham.FindAsync(id);
            if (loaiSanPham == null)
            {
                return NotFound();
            }

            var hasChildren = await _context.LoaiSanPham.AnyAsync(x => x.ParentId == id);
            if (hasChildren)
            {
                return BadRequest("Không thể xóa vì còn loại con.");
            }

            _context.LoaiSanPham.Remove(loaiSanPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool LoaiSanPhamExists(string id)
        {
            return _context.LoaiSanPham.Any(e => e.MaLoai == id);
        }

        // GET: api/LoaiSanPham/flat
        [HttpGet("flat")]
        public async Task<ActionResult<IEnumerable<object>>> GetLoaiSanPhamPhang()
        {
            var danhMuc = await _context.LoaiSanPham.ToListAsync();

            var result = from loai in danhMuc
                         join cha in danhMuc on loai.ParentId equals cha.MaLoai into gj
                         from cha in gj.DefaultIfEmpty()
                         select new
                         {
                             MaLoai = loai.MaLoai,
                             TenLoai = loai.TenLoai,
                             XuatSu = loai.XuatSu,
                             ParentId = loai.ParentId,
                             //TenLoaiCha = cha != null ? cha.TenLoai : null
                         };

            return Ok(result);
        }

    }
}
