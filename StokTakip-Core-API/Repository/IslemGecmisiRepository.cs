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

        public async Task<ICollection<IslemGecmisi>> GetIslemGecmisleri()
        {
            return await _context.IslemGecmisi_Logs.ToListAsync();
        }
    }
}