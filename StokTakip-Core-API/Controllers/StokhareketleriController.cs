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

        [HttpPost]
        public async Task<IActionResult> StokHareketiEkle([FromBody] DTOs.StokhareketiDTO yeniHareketDTO)
        {
            var islemGorenUrun = await _context.Urunler.FindAsync(yeniHareketDTO.UrunID);

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

            await _context.StokHareketleri.AddAsync(eklenecekHareket);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mesaj = "Stok hareketi başarıyla işlendi ve ürünün güncel stoğu güncellendi.",
                guncelStok = islemGorenUrun.StokAdedi,
                hareket = eklenecekHareket
            });
        }
    }
}
