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
                StokAdedi = yeniUrunDTO.StokMiktari
            };
            await _context.Urunler.AddAsync(eklenecekUrun);
            await _context.SaveChangesAsync();
            return Ok(new { Mesajlar = "OK!", urun = eklenecekUrun });
        }   
    }
}