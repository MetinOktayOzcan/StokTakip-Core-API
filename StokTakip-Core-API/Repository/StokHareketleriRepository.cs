using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Repository
{
    public class StokHareketleriRepository : IStokHareketleriRepository
    {
        private readonly stokTakipContext _context;

        public StokHareketleriRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<StokhareketiDTO>> GetStokHareketleri(int sayfa = 1, int boyut = 50)
        {
            return await _context.StokHareketleri.AsNoTracking()
                .OrderByDescending(x => x.IslemTarihi)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .Join(_context.Urunler.IgnoreQueryFilters(),
                    hareket => hareket.UrunID,
                    urun => urun.UrunId,
                    (hareket, urun) => new StokhareketiDTO
                    {
                        HareketID = hareket.HareketID,
                        UrunID = hareket.UrunID,
                        UrunAdi = urun.IsDeleted ? urun.UrunAdi + " (Silinmiş)" : urun.UrunAdi,
                        IslemTuru = hareket.IslemTuru ?? "Belirtilmedi",
                        Miktar = hareket.Miktar,
                        Aciklama = hareket.Aciklama,
                        IslemTarihi = hareket.IslemTarihi,
                        Konum = hareket.Konum
                    })
                .ToListAsync();
        }

        public async Task<bool> StokHareketiEkle(StokHareketleri hareket)
        {
            _context.StokHareketleri.Add(hareket);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}