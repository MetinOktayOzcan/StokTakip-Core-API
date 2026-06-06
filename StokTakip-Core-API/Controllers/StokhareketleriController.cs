using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StokhareketleriController : Controller
    {
        private readonly IStokHareketleriRepository _stokHareketleriRepository;
        private readonly IUrunRepository _urunRepository;

        public StokhareketleriController(IStokHareketleriRepository stokHareketleriRepository, IUrunRepository urunRepository)
        {
            _stokHareketleriRepository = stokHareketleriRepository;
            _urunRepository = urunRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StokhareketiDTO>>> GetStokHareketleri()
        {
            var hareketler = await _stokHareketleriRepository.GetStokHareketleri();

            if (hareketler == null)
                return NotFound();

            return Ok(hareketler);
        }

        [HttpPost]
        public async Task<IActionResult> StokHareketiEkle([FromBody] DTOs.StokhareketiDTO yeniHareketDTO)
        {
            var islemGorenUrun = await _urunRepository.GetUrunById(yeniHareketDTO.UrunID);

            if (islemGorenUrun == null)
            {
                return NotFound(new { mesaj = "Hata! İşlem yapmak istediğiniz ürün veritabanında bulunamadı." });
            }

            if (yeniHareketDTO.IslemTuru.ToLower() == "giriş" || yeniHareketDTO.IslemTuru.ToLower() == "giris")
            {
                islemGorenUrun.StokAdedi += yeniHareketDTO.Miktar;
            }
            else if (yeniHareketDTO.IslemTuru.ToLower() == "çıkış" || yeniHareketDTO.IslemTuru.ToLower() == "cikis")
            {
                if (islemGorenUrun.StokAdedi < yeniHareketDTO.Miktar)
                {
                    return BadRequest(new { mesaj = $"Hata! Yetersiz Stok. Depoda sadece {islemGorenUrun.StokAdedi} adet ürün var." });
                }
                islemGorenUrun.StokAdedi -= yeniHareketDTO.Miktar;
            }
            else
            {
                return BadRequest(new { mesaj = "Geçersiz İşlem Türü! Lütfen sadece 'Giriş' veya 'Çıkış' yazınız." });
            }

            var eklenecekHareket = new Models.StokHareketleri
            {
                UrunID = yeniHareketDTO.UrunID,
                IslemTuru = yeniHareketDTO.IslemTuru,
                Miktar = yeniHareketDTO.Miktar,
                Aciklama = yeniHareketDTO.Aciklama,
                IslemTarihi = DateTime.Now
            };


            await _stokHareketleriRepository.StokHareketiEkle(eklenecekHareket);

            await _urunRepository.UrunGuncelle(islemGorenUrun);

            return Ok(new
            {
                mesaj = "OK!",
                guncelStok = islemGorenUrun.StokAdedi,
                hareket = eklenecekHareket
            });
        }
    }
}