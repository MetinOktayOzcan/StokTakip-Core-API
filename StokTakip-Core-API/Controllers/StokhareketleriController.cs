using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Data;
using Microsoft.EntityFrameworkCore;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StokhareketleriController : Controller
    {
        private readonly stokTakipContext _context;

        public StokhareketleriController(stokTakipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStokHareketleri()
        {
            var StokHareketleri = await _context.StokHareketleri.ToListAsync();

            return Ok(StokHareketleri);
        }
    }
}
