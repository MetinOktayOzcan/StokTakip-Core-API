using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunlerController : ControllerBase
    {
        private readonly stokTakipContext _context;

        public UrunlerController(stokTakipContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUrunler()
        {
            var urunlerListesi = await _context.Urunler
                                         .Include(u => u.Kategori)
                                         .ToListAsync();

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
            await _context.Urunler.AddAsync(eklenecekUrun);
            await _context.SaveChangesAsync();
            return Ok(new { Mesajlar = "OK!", urun = eklenecekUrun });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UrunGuncelle(int id, [FromBody] DTOs.UrunEkleDTO guncelUrunDTO)
        {
            var guncellenecekUrun = await _context.Urunler.FindAsync(id);

            if (guncellenecekUrun == null)
            {
                return NotFound(new { mesaj = "Güncellenecek ürün bulunamadı!" }); // 404 Hatası dön
            }
            guncellenecekUrun.UrunAdi = guncelUrunDTO.UrunAdi;
            guncellenecekUrun.BirimFiyati = guncelUrunDTO.BirimFiyat;
            guncellenecekUrun.StokAdedi = guncelUrunDTO.StokMiktari;
            guncellenecekUrun.KategoriID = guncelUrunDTO.KategoriID;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "OK!", urun = guncellenecekUrun });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> UrunSil(int id)
        {
            bool stokHareketiVarMi = await _context.StokHareketleri.AnyAsync(s => s.UrunID == id);

            if (stokHareketiVarMi)
            {
                return BadRequest(new { mesaj = "Hata! Bu ürünün stok geçmişi bulunduğu için sistemden kalıcı olarak silinemez." });
            }

            var silinecekUrun = await _context.Urunler.FindAsync(id);

            if (silinecekUrun == null)
            {
                return NotFound(new { mesaj = "Silinmek istenen ürün yok" });
            }

            // 3. Ürünü veritabanından kaldır
            _context.Urunler.Remove(silinecekUrun);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Ürün sistemden silindi" });
        }

    }
}