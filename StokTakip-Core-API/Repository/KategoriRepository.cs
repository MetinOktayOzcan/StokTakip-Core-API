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

        public async Task<ICollection<Kategoriler>> GetKategoriler()
        {
            return await _context.Kategoriler.OrderBy(k => k.KategoriID).ToListAsync();
        }

        public async Task<bool> KategoriEkle(Kategoriler kategori)
        {
            await _context.Kategoriler.AddAsync(kategori);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}