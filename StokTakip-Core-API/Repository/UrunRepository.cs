using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;
using System.Xml.Linq;

namespace StokTakip_Core_API.Repository
{
    public class UrunRepository : IUrunRepository
    {
        private readonly stokTakipContext _context;

        public UrunRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Urun>> GetUrunler()
        {
            return await _context.Urunler.Include(u => u.Kategori).ToListAsync();
        }

        public async Task<bool> UrunEkle(Urun urun)
        {
            await _context.Urunler.AddAsync(urun);
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
            _context.Urunler.Remove(urun);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UrununStokHareketiVarMi(int id)
        {
            return await _context.StokHareketleri.AnyAsync(s => s.UrunID == id);
        }
    }
}