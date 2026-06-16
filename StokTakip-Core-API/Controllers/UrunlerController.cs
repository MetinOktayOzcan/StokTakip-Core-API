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
    public class UrunlerController : ControllerBase
    {
        private readonly IUrunRepository _urunRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IKategoriRepository _kategoriRepository;

        public UrunlerController(IUrunRepository urunRepository, IAuditLogService auditLogService, IDateTimeProvider dateTimeProvider, IKategoriRepository kategoriRepository)
        {
            _urunRepository = urunRepository;
            _auditLogService = auditLogService;
            _dateTimeProvider = dateTimeProvider;
            _kategoriRepository = kategoriRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUrunler([FromQuery] int sayfa = 1, [FromQuery] int boyut = 50)
        {
            sayfa = sayfa < 1 ? 1 : Math.Min(sayfa, 1000);
            boyut = boyut < 1 ? 10 : Math.Min(boyut, 100);

            var urunler = await _urunRepository.GetUrunler(sayfa, boyut);

            var sonuc = urunler.Select(u => new
            {
                u.UrunId,
                u.UrunAdi,
                KategoriAdi = u.Kategori?.KategoriAdi ?? "Kategorisiz",
                u.BirimFiyati,
                u.StokAdedi,
                u.Konum
            });

            return Ok(sonuc);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrun(int id)
        {
            var urun = await _urunRepository.GetUrunById(id);
            if (urun == null) return NotFound();

            return Ok(new
            {
                urun.UrunId,
                urun.UrunAdi,
                urun.KategoriID,
                urun.BirimFiyati,
                urun.StokAdedi,
                urun.Konum
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> UrunEkle([FromBody] UrunEkleDTO dto)
        {
            var kategori = await _kategoriRepository.GetKategoriById(dto.KategoriID);
            if (kategori == null) return BadRequest(new { Mesaj = "Belirtilen kategori bulunamadı." });

            var urun = new Urun
            {
                UrunAdi = dto.UrunAdi,
                KategoriID = dto.KategoriID,
                BirimFiyati = dto.BirimFiyati,
                StokAdedi = dto.StokAdedi,
                Konum = dto.Konum,
                EklenmeTarihi = _dateTimeProvider.Now,
                IsDeleted = false
            };

            var basarili = await _urunRepository.UrunEkle(urun);
            if (!basarili) return BadRequest("Ürün kaydedilirken bir hata oluştu.");

            await _auditLogService.LogOlusturAsync("Ürün Ekleme", $"'{dto.UrunAdi}' eklendi.");

            return CreatedAtAction(nameof(GetUrun), new { id = urun.UrunId }, urun);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<IActionResult> UrunGuncelle(int id, [FromBody] UrunGuncelleDTO dto)
        {
            var kategori = await _kategoriRepository.GetKategoriById(dto.KategoriID);
            if (kategori == null) return BadRequest(new { Mesaj = "Belirtilen kategori bulunamadı." });

            var urun = await _urunRepository.GetUrunById(id);
            if (urun == null) return NotFound();

            urun.UrunAdi = dto.UrunAdi;
            urun.KategoriID = dto.KategoriID;
            urun.BirimFiyati = dto.BirimFiyati;
            urun.Konum = dto.Konum;

            var basarili = await _urunRepository.UrunGuncelle(urun);
            if (!basarili) return BadRequest("Ürün güncellenemedi.");

            await _auditLogService.LogOlusturAsync("Ürün Güncelleme", $"'{urun.UrunAdi}' güncellendi.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UrunSil(int id)
        {
            var urun = await _urunRepository.GetUrunById(id);
            if (urun == null) return NotFound();

            if (urun.StokAdedi > 0)
                return BadRequest($"Bu üründen stokta {urun.StokAdedi} adet mevcut. Silmeden önce stok sıfırlanmalıdır.");

            urun.IsDeleted = true;
            var basarili = await _urunRepository.UrunGuncelle(urun);
            if (!basarili) return BadRequest("Silme işlemi gerçekleştirilemedi.");

            await _auditLogService.LogOlusturAsync("Ürün Silme", $"'{urun.UrunAdi}' sistemden kaldırıldı.");

            return NoContent();
        }
    }
}