using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UrunlerController : ControllerBase
    {
        private readonly IUrunRepository _urunRepository;
        private readonly stokTakipContext _context;

        public UrunlerController(IUrunRepository urunRepository, stokTakipContext context)
        {
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
        public async Task<IActionResult> GetUrunler()
        {
            var urunlerListesi = await _urunRepository.GetUrunler();

            var donulecekDTO = urunlerListesi.Select(u => new DTOs.UrunEkleDTO
            {
                UrunID = u.UrunId,
                UrunAdi = u.UrunAdi,
                BirimFiyat = u.BirimFiyati,
                StokMiktari = u.StokAdedi,
                Konum = u.Konum,
                KategoriAdi = u.Kategori != null ? u.Kategori.KategoriAdi : "Kategorisiz"
            }).ToList();

            return Ok(donulecekDTO);
        }

        [HttpPost]
        public async Task<IActionResult> UrunEkle([FromBody] DTOs.UrunEkleDTO yeniUrunDTO)
        {
            var eklenecekUrun = new Models.Urun
            {
                UrunAdi = yeniUrunDTO.UrunAdi,
                BirimFiyati = yeniUrunDTO.BirimFiyat,
                StokAdedi = yeniUrunDTO.StokMiktari,
                KategoriID = yeniUrunDTO.KategoriID,
                Konum = yeniUrunDTO.Konum,
                EklenmeTarihi = DateTime.UtcNow.AddHours(3)
            };

            bool kaydedildiMi = await _urunRepository.UrunEkle(eklenecekUrun);

            if (kaydedildiMi)
            {
                await LogOlustur("Ürün Ekleme", $"{eklenecekUrun.UrunAdi} sisteme eklendi. Fiyat: {eklenecekUrun.BirimFiyati}, Stok: {eklenecekUrun.StokAdedi}");
                return Ok(new { Mesajlar = "OK!", urun = eklenecekUrun });
            }
            return BadRequest(new { Mesajlar = "Hata! Ürün kaydedilemedi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UrunGuncelle(int id, [FromBody] DTOs.UrunEkleDTO guncelUrunDTO)
        {
            var guncellenecekUrun = await _urunRepository.GetUrunById(id);

            if (guncellenecekUrun == null)
                return NotFound(new { mesaj = "Güncellenecek ürün bulunamadı!" });

            var degisiklikler = new List<string>();

            if (guncellenecekUrun.BirimFiyati != guncelUrunDTO.BirimFiyat)
                degisiklikler.Add($"Fiyat: {guncellenecekUrun.BirimFiyati} ➔ {guncelUrunDTO.BirimFiyat} TL");

            if (guncellenecekUrun.StokAdedi != guncelUrunDTO.StokMiktari)
                degisiklikler.Add($"Stok: {guncellenecekUrun.StokAdedi} ➔ {guncelUrunDTO.StokMiktari}");

            if (guncellenecekUrun.Konum != guncelUrunDTO.Konum)
                degisiklikler.Add($"Konum: {guncellenecekUrun.Konum} ➔ {guncelUrunDTO.Konum}");

            string logDetayi = degisiklikler.Any()
                ? $"'{guncellenecekUrun.UrunAdi}' güncellendi. Değişenler: {string.Join(", ", degisiklikler)}"
                : $"'{guncellenecekUrun.UrunAdi}' güncellendi ama hiçbir değer değiştirilmedi.";

            guncellenecekUrun.UrunAdi = guncelUrunDTO.UrunAdi;
            guncellenecekUrun.BirimFiyati = guncelUrunDTO.BirimFiyat;
            guncellenecekUrun.StokAdedi = guncelUrunDTO.StokMiktari;
            guncellenecekUrun.KategoriID = guncelUrunDTO.KategoriID;
            guncellenecekUrun.Konum = guncelUrunDTO.Konum;

            bool guncellendiMi = await _urunRepository.UrunGuncelle(guncellenecekUrun);

            if (guncellendiMi)
            {
                await LogOlustur("Ürün Güncelleme", logDetayi);
                return Ok(new { mesaj = "OK!", urun = guncellenecekUrun });
            }
            return BadRequest(new { mesaj = "Hata! Ürün güncellenemedi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UrunSil(int id)
        {
            bool stokHareketiVarMi = await _urunRepository.UrununStokHareketiVarMi(id);

            if (stokHareketiVarMi)
                return BadRequest(new { mesaj = "Bu ürünün stok geçmişi bulunduğu için sistemden kalıcı olarak silinemez." });

            var silinecekUrun = await _urunRepository.GetUrunById(id);

            if (silinecekUrun == null)
                return NotFound(new { mesaj = "Silinmek istenen ürün yok." });

            string silinenUrunAdi = silinecekUrun.UrunAdi;
            bool silindiMi = await _urunRepository.UrunSil(silinecekUrun);

            if (silindiMi)
            {
                await LogOlustur("Ürün Silme", $"'{silinenUrunAdi}' adlı ürün silindi.");
                return Ok(new { mesaj = "Ürün sistemden silindi" });
            }
            return BadRequest(new { mesaj = "Hata! Ürün silinemedi." });
        }
    }
}