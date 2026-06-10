using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StokhareketleriController : ControllerBase
    {
        private readonly IStokHareketleriRepository _stokHareketleriRepository;
        private readonly IUrunRepository _urunRepository;
        private readonly stokTakipContext _context;

        public StokhareketleriController(IStokHareketleriRepository stokHareketleriRepository, IUrunRepository urunRepository, stokTakipContext context)
        {
            _stokHareketleriRepository = stokHareketleriRepository;
            _urunRepository = urunRepository;
            _context = context;
        }

        private async Task LogOlustur(string islemTipi, string detay)
        {
            var aktifKullaniciAdi = User.Identity?.Name;
            var islemYapanAdSoyad = "Sistem";

            if (!string.IsNullOrEmpty(aktifKullaniciAdi))
            {
                var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == aktifKullaniciAdi);
                islemYapanAdSoyad = !string.IsNullOrWhiteSpace(kullanici?.AdSoyad) ? kullanici.AdSoyad : aktifKullaniciAdi;
            }

            var log = new IslemGecmisi
            {
                IslemTarihi = DateTime.UtcNow.AddHours(3),
                Kullanici = islemYapanAdSoyad,
                IslemTipi = islemTipi,
                Detay = detay
            };

            _context.IslemGecmisi.Add(log);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StokhareketiDTO>>> GetStokHareketleri()
        {
            var hareketler = await _stokHareketleriRepository.GetStokHareketleri();
            if (hareketler == null) return NotFound();
            return Ok(hareketler);
        }

        [HttpPost]
        public async Task<IActionResult> StokHareketiEkle([FromBody] StokhareketiDTO yeniHareketDTO)
        {
            var islemGorenUrun = await _urunRepository.GetUrunById(yeniHareketDTO.UrunID);

            if (islemGorenUrun == null)
                return NotFound(new { mesaj = "İşlem yapmak istediğiniz ürün bulunamadı." });

            var islemTuru = yeniHareketDTO.IslemTuru.ToLower();

            if (islemTuru == "giriş" || islemTuru == "giris")
            {
                islemGorenUrun.StokAdedi += yeniHareketDTO.Miktar;
            }
            else if (islemTuru == "çıkış" || islemTuru == "cikis")
            {
                if (islemGorenUrun.StokAdedi < yeniHareketDTO.Miktar)
                    return BadRequest(new { mesaj = $"Yetersiz Stok! Depoda sadece {islemGorenUrun.StokAdedi} adet ürün var." });

                islemGorenUrun.StokAdedi -= yeniHareketDTO.Miktar;
            }
            else
            {
                return BadRequest(new { mesaj = "Lütfen sadece 'Giriş' veya 'Çıkış' yazınız." });
            }

            var eklenecekHareket = new StokHareketleri
            {
                UrunID = yeniHareketDTO.UrunID,
                IslemTuru = yeniHareketDTO.IslemTuru,
                Miktar = yeniHareketDTO.Miktar,
                Aciklama = yeniHareketDTO.Aciklama,
                Konum = yeniHareketDTO.Konum,
                IslemTarihi = DateTime.UtcNow.AddHours(3)
            };

            await _stokHareketleriRepository.StokHareketiEkle(eklenecekHareket);
            await _urunRepository.UrunGuncelle(islemGorenUrun);

            await LogOlustur("Stok Hareketi", $"'{islemGorenUrun.UrunAdi}' ürünü için {eklenecekHareket.Miktar} adet {eklenecekHareket.IslemTuru}. Konum: {yeniHareketDTO.Konum ?? "Belirtilmedi"}");

            return Ok(new { mesaj = "OK!", guncelStok = islemGorenUrun.StokAdedi, hareket = eklenecekHareket });
        }
    }
}