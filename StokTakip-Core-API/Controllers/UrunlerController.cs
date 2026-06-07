using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UrunlerController : ControllerBase
    {
        private readonly IUrunRepository _urunRepository;

        public UrunlerController(IUrunRepository urunRepository)
        {
            _urunRepository = urunRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUrunler()
        {
            var urunlerListesi = await _urunRepository.GetUrunler();

            var donulecekDTO = urunlerListesi.Select(u => new DTOs.UrunEkleDTO
            {
                UrunID = u.UrunId,
                UrunAdi = u.UrunAdi,
                BirimFiyat = u.BirimFiyati,
                StokMiktari = u.StokAdedi,
                KategoriAdi = u.Kategori != null ? u.Kategori.KategoriAdi : "Kategorisiz"
            }).ToList();

            return Ok(donulecekDTO);
        }

        [HttpPost]
        public async Task<IActionResult> UrunEkle([FromBody] DTOs.UrunEkleDTO yeniUrunDTO)
        {
            var eklenecekUrun = new Models.urun
            {
                UrunAdi = yeniUrunDTO.UrunAdi,
                BirimFiyati = yeniUrunDTO.BirimFiyat,
                StokAdedi = yeniUrunDTO.StokMiktari,
                KategoriID = yeniUrunDTO.KategoriID,
                EklenmeTarihi = DateTime.Now
            };

            bool kaydedildiMi = await _urunRepository.UrunEkle(eklenecekUrun);

            if (kaydedildiMi)
            {
                return Ok(new { Mesajlar = "OK!", urun = eklenecekUrun });
            }
            return BadRequest(new { Mesajlar = "Hata! Ürün kaydedilemedi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UrunGuncelle(int id, [FromBody] DTOs.UrunEkleDTO guncelUrunDTO)
        {
            var guncellenecekUrun = await _urunRepository.GetUrunById(id);

            if (guncellenecekUrun == null)
            {
                return NotFound(new { mesaj = "Güncellenecek ürün bulunamadı!" });
            }

            guncellenecekUrun.UrunAdi = guncelUrunDTO.UrunAdi;
            guncellenecekUrun.BirimFiyati = guncelUrunDTO.BirimFiyat;
            guncellenecekUrun.StokAdedi = guncelUrunDTO.StokMiktari;
            guncellenecekUrun.KategoriID = guncelUrunDTO.KategoriID;

            bool guncellendiMi = await _urunRepository.UrunGuncelle(guncellenecekUrun);

            if (guncellendiMi)
            {
                return Ok(new { mesaj = "OK!", urun = guncellenecekUrun });
            }
            return BadRequest(new { mesaj = "Hata! Ürün güncellenemedi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UrunSil(int id)
        {
            bool stokHareketiVarMi = await _urunRepository.UrununStokHareketiVarMi(id);

            if (stokHareketiVarMi)
            {
                return BadRequest(new { mesaj = "Hata! Bu ürünün stok geçmişi bulunduğu için sistemden kalıcı olarak silinemez." });
            }

            var silinecekUrun = await _urunRepository.GetUrunById(id);

            if (silinecekUrun == null)
            {
                return NotFound(new { mesaj = "Silinmek istenen ürün yok" });
            }

            bool silindiMi = await _urunRepository.UrunSil(silinecekUrun);

            if (silindiMi)
            {
                return Ok(new { mesaj = "Ürün sistemden silindi" });
            }
            return BadRequest(new { mesaj = "Hata! Ürün silinemedi." });
        }
    }
}