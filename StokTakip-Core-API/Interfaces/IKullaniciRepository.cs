using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IKullaniciRepository
    {
        Task<ICollection<Kullanici>> GetKullanicilar(int sayfa = 1, int boyut = 50);
        Task<Kullanici?> GetKullaniciById(int id);
        Task<Kullanici?> GetKullaniciByKullaniciAdi(string kullaniciAdi);
        Task<bool> KullaniciEkle(Kullanici kullanici);
        Task<bool> KullaniciGuncelle(Kullanici kullanici);
        Task<bool> KullaniciSil(Kullanici kullanici);
        Task<bool> KullaniciMevcutMu(string kullaniciAdi);
        Task<Kullanici?> GetKullaniciByRefreshTokenHash(string refreshTokenHash);
    }
}