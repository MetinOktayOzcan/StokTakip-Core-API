using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;
using StokTakip_Core_API.Services;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KategorilerController : ControllerBase
    {
        private readonly IKategoriRepository _kategoriRepository;
        private readonly IAuditLogService _auditLogService;

        public KategorilerController(IKategoriRepository kategoriRepository, IAuditLogService auditLogService)
        {
            _kategoriRepository = kategoriRepository;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int sayfa = 1, [FromQuery] int boyut = 50)
        {
            sayfa = sayfa < 1 ? 1 : Math.Min(sayfa, 1000);
            boyut = boyut < 1 ? 10 : Math.Min(boyut, 100);

            var kategoriler = await _kategoriRepository.GetKategoriler(sayfa, boyut);
            return Ok(kategoriler.Select(k => new { k.KategoriID, k.KategoriAdi }));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> Post([FromBody] KategoriEkleDTO dto)
        {
            var kategori = new Kategoriler { KategoriAdi = dto.KategoriAdi };

            if (!await _kategoriRepository.KategoriEkle(kategori))
                return BadRequest("Kategori eklenemedi.");

            await _auditLogService.LogOlusturAsync("Kategori Ekleme", $"'{dto.KategoriAdi}' eklendi.");
            return CreatedAtAction(nameof(Get), new { id = kategori.KategoriID }, kategori);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> Put(int id, [FromBody] KategoriEkleDTO dto)
        {
            var kategori = await _kategoriRepository.GetKategoriById(id);
            if (kategori == null) return NotFound();

            kategori.KategoriAdi = dto.KategoriAdi;

            if (!await _kategoriRepository.KategoriGuncelle(kategori))
                return BadRequest("Güncelleme işlemi başarısız oldu.");

            await _auditLogService.LogOlusturAsync("Kategori Güncelleme", $"'{kategori.KategoriAdi}' güncellendi.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _kategoriRepository.GetKategoriById(id);
            if (kategori == null) return NotFound();

            if (await _kategoriRepository.KategoriyeAitUrunVarMi(id))
                return BadRequest("Bu kategoriye ait aktif ürünler bulunduğu için silinemez.");

            var silinenAd = kategori.KategoriAdi;

            if (!await _kategoriRepository.KategoriSil(kategori))
                return BadRequest("Kategori silinirken bir hata oluştu.");

            await _auditLogService.LogOlusturAsync("Kategori Silme", $"'{silinenAd}' silindi.");
            return NoContent();
        }
    }
}