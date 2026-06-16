using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Repository
{
    public class KategoriRepository : IKategoriRepository
    {
        private readonly stokTakipContext _context;

        public KategoriRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Kategoriler>> GetKategoriler(int sayfa = 1, int boyut = 50)
        {
            return await _context.Kategoriler
                .AsNoTracking()
                .OrderBy(k => k.KategoriID)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .ToListAsync();
        }

        public async Task<Kategoriler?> GetKategoriById(int id)
        {
            return await _context.Kategoriler.FindAsync(id);
        }

        public async Task<bool> KategoriEkle(Kategoriler kategori)
        {
            _context.Kategoriler.Add(kategori);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KategoriGuncelle(Kategoriler kategori)
        {
            _context.Kategoriler.Update(kategori);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KategoriSil(Kategoriler kategori)
        {
            _context.Kategoriler.Remove(kategori);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KategoriyeAitUrunVarMi(int id)
        {
            return await _context.Urunler.IgnoreQueryFilters().AnyAsync(u => u.KategoriID == id);
        }
    }
}