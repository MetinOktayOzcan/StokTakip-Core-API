using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Interfaces;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Yonetici")]
    public class IslemGecmisiController : ControllerBase
    {
        private readonly IIslemGecmisiRepository _islemGecmisiRepository;

        public IslemGecmisiController(IIslemGecmisiRepository islemGecmisiRepository)
        {
            _islemGecmisiRepository = islemGecmisiRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int sayfa = 1, [FromQuery] int boyut = 50)
        {
            sayfa = sayfa < 1 ? 1 : Math.Min(sayfa, 1000);
            boyut = boyut < 1 ? 10 : Math.Min(boyut, 100);

            var gecmisVerisi = await _islemGecmisiRepository.GetIslemGecmisleri(sayfa, boyut);

            var sonuc = gecmisVerisi.Select(x => new
            {
                x.LogID,
                x.Kullanici,
                x.IslemTipi,
                x.Detay,
                x.IslemTarihi
            });

            return Ok(sonuc);
        }
    }
}