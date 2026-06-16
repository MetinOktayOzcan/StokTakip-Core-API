using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;
using StokTakip_Core_API.Services;
using System.Security.Claims;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Yonetici")]
    public class KullanicilarController : ControllerBase
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IAuditLogService _auditLogService;

        public KullanicilarController(IKullaniciRepository kullaniciRepository, IAuditLogService auditLogService)
        {
            _kullaniciRepository = kullaniciRepository;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int sayfa = 1, [FromQuery] int boyut = 50)
        {
            sayfa = sayfa < 1 ? 1 : Math.Min(sayfa, 1000);
            boyut = boyut < 1 ? 10 : Math.Min(boyut, 100);

            var kullanicilar = await _kullaniciRepository.GetKullanicilar(sayfa, boyut);
            return Ok(kullanicilar.Select(k => new { k.KullaniciID, k.KullaniciAdi, k.AdSoyad, k.Rol }));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] KullaniciDTO dto)
        {
            if (await _kullaniciRepository.KullaniciMevcutMu(dto.KullaniciAdi))
                return BadRequest(new { Mesaj = "Bu kullanıcı adı zaten kullanılıyor." });

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = dto.KullaniciAdi,
                AdSoyad = dto.AdSoyad,
                Rol = dto.Rol,
                SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre)
            };

            if (!await _kullaniciRepository.KullaniciEkle(yeniKullanici))
                return BadRequest(new { Mesaj = "Kullanıcı eklenirken bir hata oluştu." });

            await _auditLogService.LogOlusturAsync("Kullanıcı Ekleme", $"'{dto.AdSoyad}' ({dto.Rol}) sisteme eklendi.");
            return CreatedAtAction(nameof(Get), new { id = yeniKullanici.KullaniciID }, new { Mesaj = "Kullanıcı başarıyla eklendi." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] KullaniciDTO dto)
        {
            var kullanici = await _kullaniciRepository.GetKullaniciById(id);
            if (kullanici == null) return NotFound(new { Mesaj = "Kullanıcı bulunamadı." });

            var tokenKullaniciId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (kullanici.KullaniciAdi.ToLower() == "admin")
            {
                if (kullanici.KullaniciID.ToString() != tokenKullaniciId)
                    return BadRequest(new { Mesaj = "Sistem ana yöneticisinin bilgileri başka bir yönetici tarafından değiştirilemez." });

                if (dto.KullaniciAdi.ToLower() != "admin")
                    return BadRequest(new { Mesaj = "Sistem ana yöneticisinin kullanıcı adı değiştirilemez." });

                if (dto.Rol != "Admin")
                    return BadRequest(new { Mesaj = "Sistem ana yöneticisinin yetkisi düşürülemez." });
            }

            if (kullanici.KullaniciAdi != dto.KullaniciAdi && await _kullaniciRepository.KullaniciMevcutMu(dto.KullaniciAdi))
                return BadRequest(new { Mesaj = "Bu kullanıcı adı başka bir kullanıcıya ait." });

            kullanici.AdSoyad = dto.AdSoyad;
            kullanici.KullaniciAdi = dto.KullaniciAdi;
            kullanici.Rol = dto.Rol;

            if (!string.IsNullOrWhiteSpace(dto.Sifre))
            {
                kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);
                kullanici.RefreshTokenHash = null;
                kullanici.RefreshTokenExpiryTime = null;
            }

            if (!await _kullaniciRepository.KullaniciGuncelle(kullanici))
                return BadRequest(new { Mesaj = "Kullanıcı güncellenemedi." });

            await _auditLogService.LogOlusturAsync("Kullanıcı Güncelleme", $"'{dto.AdSoyad}' güncellendi. Yeni Rol: {dto.Rol}");
            return Ok(new { Mesaj = "Kullanıcı bilgileri güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var kullanici = await _kullaniciRepository.GetKullaniciById(id);
            if (kullanici == null) return NotFound(new { Mesaj = "Kullanıcı bulunamadı." });

            var tokenKullaniciId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (kullanici.KullaniciID.ToString() == tokenKullaniciId)
                return BadRequest(new { Mesaj = "Kendi hesabınızı silemezsiniz." });

            if (kullanici.KullaniciAdi.ToLower() == "admin")
                return BadRequest(new { Mesaj = "Sistem varsayılan ana yöneticisi silinemez." });

            var silinenAd = kullanici.AdSoyad;

            if (!await _kullaniciRepository.KullaniciSil(kullanici))
                return BadRequest(new { Mesaj = "Kullanıcı silinemedi." });

            await _auditLogService.LogOlusturAsync("Kullanıcı Silme", $"'{silinenAd}' silindi.");
            return Ok(new { Mesaj = "Kullanıcı başarıyla silindi." });
        }
    }
}