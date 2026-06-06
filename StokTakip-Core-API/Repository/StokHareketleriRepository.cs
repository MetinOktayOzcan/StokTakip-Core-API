using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Repository
{
    public class StokHareketleriRepository : IStokHareketleriRepository
    {
        private readonly stokTakipContext _context;

        public StokHareketleriRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<StokhareketiDTO>> GetStokHareketleri()
        {
            return await _context.StokHareketleri
                .Join(
                    _context.Urunler,
                    hareket => hareket.UrunID,
                    urun => urun.UrunId,
                    (hareket, urun) => new StokhareketiDTO
                    {
                        HareketID = hareket.HareketID,
                        UrunID = hareket.UrunID,
                        UrunAdi = urun.UrunAdi,
                        IslemTuru = hareket.IslemTuru,
                        Miktar = hareket.Miktar,
                        Aciklama = hareket.Aciklama,
                        IslemTarihi = hareket.IslemTarihi
                    }
                )
                .OrderByDescending(x => x.IslemTarihi)
                .ToListAsync();
        }

        public async Task<bool> StokHareketiEkle(StokHareketleri hareket)
        {
            await _context.StokHareketleri.AddAsync(hareket);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}