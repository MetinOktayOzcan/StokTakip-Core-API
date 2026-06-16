using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IKategoriRepository
    {
        Task<ICollection<Kategoriler>> GetKategoriler(int sayfa = 1, int boyut = 50);
        Task<Kategoriler?> GetKategoriById(int id);
        Task<bool> KategoriEkle(Kategoriler kategori);
        Task<bool> KategoriGuncelle(Kategoriler kategori);
        Task<bool> KategoriSil(Kategoriler kategori);
        Task<bool> KategoriyeAitUrunVarMi(int id);
    }
}