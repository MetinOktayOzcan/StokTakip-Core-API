using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class KategorilerController : Controller
    {
        private readonly IKategoriRepository _kategoriRepository;

        public KategorilerController(IKategoriRepository kategoriRepository)
        {
            _kategoriRepository = kategoriRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetKategoriler()
        {
            var Kategoriler = await _kategoriRepository.GetKategoriler();

            return Ok(Kategoriler);
        }

        [HttpPost]
        public async Task<IActionResult> KategoriEkle([FromBody] DTOs.KategoriEkleDTO yeniKategoriDTO)
        {
            var eklenecekKategori = new Models.Kategoriler
            {
                KategoriAdi = yeniKategoriDTO.KategoriAdi
            };
            bool kaydedildiMi = await _kategoriRepository.KategoriEkle(eklenecekKategori);

            if (kaydedildiMi)
            {
                return Ok(new { Mesajlar = "OK!", kategori = eklenecekKategori });
            }

            return BadRequest(new { Mesajlar = "Hata!!! Kategori kaydedilemedi." });
        }
    }
}
