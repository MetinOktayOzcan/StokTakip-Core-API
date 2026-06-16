using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Repository
{
    public class UrunRepository : IUrunRepository
    {
        private readonly stokTakipContext _context;

        public UrunRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Urun>> GetUrunler(int sayfa = 1, int boyut = 50)
        {
            return await _context.Urunler
                .AsNoTracking()
                .Include(u => u.Kategori)
                .OrderBy(u => u.UrunId)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .ToListAsync();
        }

        public async Task<bool> UrunEkle(Urun urun)
        {
            _context.Urunler.Add(urun);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Urun?> GetUrunById(int id)
        {
            return await _context.Urunler.FindAsync(id);
        }

        public async Task<bool> UrunGuncelle(Urun urun)
        {
            _context.Urunler.Update(urun);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UrunSil(Urun urun)
        {
            urun.IsDeleted = true;
            _context.Urunler.Update(urun);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UrununStokHareketiVarMi(int id)
        {
            return await _context.StokHareketleri.AnyAsync(s => s.UrunID == id);
        }
    }
}