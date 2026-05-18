using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class KategorilerController : Controller
    {
        private readonly stokTakipContext _context;

        public KategorilerController(stokTakipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetKategoriler()
        {
            var Kategoriler = await _context.Kategoriler.ToListAsync();

            return Ok(Kategoriler);
        }
    }
}
