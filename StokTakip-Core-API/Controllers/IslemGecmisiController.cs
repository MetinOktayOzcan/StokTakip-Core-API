using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IslemGecmisiController : Controller
    {
        private readonly IIslemGecmisiRepository _islemGecmisiRepository;

        public IslemGecmisiController(IIslemGecmisiRepository islemGecmisiRepository)
        {
            _islemGecmisiRepository = islemGecmisiRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetIslemGecmisleri()
        {
            var IslemGecmisleri = await _islemGecmisiRepository.GetIslemGecmisleri();
            return Ok(IslemGecmisleri);
        }
    }
}