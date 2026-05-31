using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
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

        public async Task<ICollection<StokHareketleri>> GetStokHareketleri()
        {
            return await _context.StokHareketleri.ToListAsync();
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