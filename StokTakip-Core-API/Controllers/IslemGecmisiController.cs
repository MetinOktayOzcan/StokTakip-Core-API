using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Data;
using Microsoft.EntityFrameworkCore;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IslemGecmisiController : Controller
    {
        private readonly stokTakipContext _context;

        public IslemGecmisiController(stokTakipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetIslemGecmisleri()
        {
            var IslemGecmisleri = await _context.IslemGecmisi_Logs.ToListAsync();

            return Ok(IslemGecmisleri);
        }
    }
}
