using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Models;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KategorilerController : ControllerBase
    {
        private readonly stokTakipContext _context;

        public KategorilerController(stokTakipContext context)
        {
            _context = context;
        }

        private async Task LogOlustur(string islemTipi, string detay)
        {
            var aktifKullaniciAdi = User.Identity?.Name;
            var islemYapanAdSoyad = "Sistem";

            if (!string.IsNullOrEmpty(aktifKullaniciAdi))
            {
                var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == aktifKullaniciAdi);
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
        public async Task<IActionResult> GetKategoriler()
        {
            var kategoriler = await _context.Kategoriler.ToListAsync();
            return Ok(kategoriler);
        }

        [HttpPost]
        public async Task<IActionResult> KategoriEkle([FromBody] KategoriEkleDTO dto)
        {
            var kategori = new Kategoriler { KategoriAdi = dto.KategoriAdi };
            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();

            await LogOlustur("Kategori Ekleme", $"'{dto.KategoriAdi}' adlı kategori eklendi.");
            return Ok(new { mesaj = "Kategori eklendi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> KategoriGuncelle(int id, [FromBody] KategoriEkleDTO dto)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null) return NotFound(new { mesaj = "Kategori bulunamadı." });

            var eskiAd = kategori.KategoriAdi;
            kategori.KategoriAdi = dto.KategoriAdi;
            await _context.SaveChangesAsync();

            await LogOlustur("Kategori Güncelleme", $"'{eskiAd}' ➔ '{dto.KategoriAdi}' olarak güncellendi.");
            return Ok(new { mesaj = "Kategori güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> KategoriSil(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null) return NotFound(new { mesaj = "Kategori bulunamadı." });

            var silinenAd = kategori.KategoriAdi;
            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();

            await LogOlustur("Kategori Silme", $"'{silinenAd}' adlı kategori silindi.");
            return Ok(new { mesaj = "Kategori silindi." });
        }
    }
}