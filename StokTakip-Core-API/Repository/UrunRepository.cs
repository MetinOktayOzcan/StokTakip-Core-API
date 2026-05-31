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

        public async Task<ICollection<urun>> GetUrunler()
        {
            return await _context.Urunler.Include(u => u.Kategori).ToListAsync();
        }

        public async Task<bool> UrunEkle(urun urun)
        {
            await _context.Urunler.AddAsync(urun);
            return await Save();
        }

        public async Task<urun> GetUrunById(int id)
        {
            return await _context.Urunler.FindAsync(id);
        }

        public async Task<bool> UrunGuncelle(urun urun)
        {
            _context.Urunler.Update(urun);
            return await Save();
        }

        public async Task<bool> UrunSil(urun urun)
        {
            _context.Urunler.Remove(urun);
            return await Save();
        }

        public async Task<bool> UrununStokHareketiVarMi(int id)
        {
            return await _context.StokHareketleri.AnyAsync(s => s.UrunID == id);
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}