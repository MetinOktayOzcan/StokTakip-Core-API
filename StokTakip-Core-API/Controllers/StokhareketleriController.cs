using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StokhareketleriController : ControllerBase
    {
        private readonly IStokHareketleriRepository _stokRepository;
        private readonly IStokHareketService _stokHareketService;

        public StokhareketleriController(IStokHareketleriRepository stokRepository, IStokHareketService stokHareketService)
        {
            _stokRepository = stokRepository;
            _stokHareketService = stokHareketService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> Get([FromQuery] int sayfa = 1, [FromQuery] int boyut = 50)
        {
            sayfa = sayfa < 1 ? 1 : Math.Min(sayfa, 1000);
            boyut = boyut < 1 ? 10 : Math.Min(boyut, 100);

            var hareketler = await _stokRepository.GetStokHareketleri(sayfa, boyut);
            return Ok(hareketler);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> Post([FromBody] StokHareketiEkleDTO dto)
        {
            var (basariliMi, mesaj) = await _stokHareketService.StokHareketiIsleAsync(dto);

            if (!basariliMi)
                return BadRequest(new { Mesaj = mesaj });

            return Ok(new { Mesaj = mesaj });
        }
    }
}