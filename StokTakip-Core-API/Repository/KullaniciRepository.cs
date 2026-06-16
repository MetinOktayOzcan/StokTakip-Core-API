using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Repository
{
    public class KullaniciRepository : IKullaniciRepository
    {
        private readonly stokTakipContext _context;

        public KullaniciRepository(stokTakipContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Kullanici>> GetKullanicilar(int sayfa = 1, int boyut = 50)
        {
            return await _context.Kullanicilar
                .AsNoTracking()
                .OrderBy(k => k.KullaniciID)
                .Skip((sayfa - 1) * boyut)
                .Take(boyut)
                .ToListAsync();
        }

        public async Task<Kullanici?> GetKullaniciById(int id)
        {
            return await _context.Kullanicilar.FindAsync(id);
        }

        public async Task<Kullanici?> GetKullaniciByKullaniciAdi(string kullaniciAdi)
        {
            return await _context.Kullanicilar.AsNoTracking().FirstOrDefaultAsync(x => x.KullaniciAdi == kullaniciAdi);
        }

        public async Task<bool> KullaniciEkle(Kullanici kullanici)
        {
            await _context.Kullanicilar.AddAsync(kullanici);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KullaniciGuncelle(Kullanici kullanici)
        {
            _context.Kullanicilar.Update(kullanici);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KullaniciSil(Kullanici kullanici)
        {
            _context.Kullanicilar.Remove(kullanici);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> KullaniciMevcutMu(string kullaniciAdi)
        {
            return await _context.Kullanicilar.AnyAsync(x => x.KullaniciAdi == kullaniciAdi);
        }

        public async Task<Kullanici?> GetKullaniciByRefreshTokenHash(string refreshTokenHash)
        {
            return await _context.Kullanicilar
                .FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshTokenHash);
        }
    }
}