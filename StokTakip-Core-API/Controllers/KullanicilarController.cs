using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KullanicilarController : ControllerBase
    {
        private readonly stokTakipContext _context;

        public KullanicilarController(stokTakipContext context)
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
        public async Task<IActionResult> GetKullanicilar()
        {
            var kullanicilar = await _context.Kullanicilar
                .Select(k => new { k.KullaniciID, k.KullaniciAdi, k.AdSoyad, k.Rol })
                .ToListAsync();
            return Ok(kullanicilar);
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciEkle([FromBody] KullaniciDTO veri)
        {
            if (await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == veri.KullaniciAdi))
                return BadRequest(new { mesaj = "Bu kullanıcı adı kullanılıyor." });

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = veri.KullaniciAdi,
                AdSoyad = veri.AdSoyad,
                Rol = veri.Rol,
                SifreHash = BCrypt.Net.BCrypt.HashPassword(veri.Sifre)
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            await LogOlustur("Kullanıcı Ekleme", $"'{veri.AdSoyad}' ({veri.Rol}) sisteme eklendi.");
            return Ok(new { mesaj = "Kullanıcı eklendi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> KullaniciGuncelle(int id, [FromBody] KullaniciDTO veri)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null) return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

            if (kullanici.KullaniciAdi != veri.KullaniciAdi &&
                await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == veri.KullaniciAdi))
                return BadRequest(new { mesaj = "Bu kullanıcı adı başkasına ait." });

            kullanici.AdSoyad = veri.AdSoyad;
            kullanici.KullaniciAdi = veri.KullaniciAdi;
            kullanici.Rol = veri.Rol;

            if (!string.IsNullOrWhiteSpace(veri.Sifre))
            {
                kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(veri.Sifre);
            }

            await _context.SaveChangesAsync();
            await LogOlustur("Kullanıcı Güncelleme", $"'{veri.AdSoyad}' güncellendi. Yeni Rol: {veri.Rol}");
            return Ok(new { mesaj = "Kullanıcı güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> KullaniciSil(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null) return NotFound(new { mesaj = "Kullanıcı bulunamadı." });

            var silinen = kullanici.AdSoyad;
            _context.Kullanicilar.Remove(kullanici);
            await _context.SaveChangesAsync();

            await LogOlustur("Kullanıcı Silme", $"'{silinen}' silindi.");
            return Ok(new { mesaj = "Kullanıcı silindi." });
        }
    }

    public class KullaniciDTO
    {
        public int Id { get; set; }
        public required string KullaniciAdi { get; set; }
        public required string AdSoyad { get; set; }
        public required string Rol { get; set; }
        public string? Sifre { get; set; }
    }
}