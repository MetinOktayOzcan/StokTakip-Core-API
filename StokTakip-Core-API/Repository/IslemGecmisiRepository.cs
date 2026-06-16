using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Repository
{
    public class IslemGecmisiRepository : IIslemGecmisiRepository
    {
        private readonly stokTakipContext _context;

        public IslemGecmisiRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<IslemGecmisi>> GetIslemGecmisleri(int sayfa = 1, int boyut = 50)
        {
            return await _context.IslemGecmisi
                .AsNoTracking()
                .OrderByDescending(x => x.IslemTarihi)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .ToListAsync();
        }
    }
}