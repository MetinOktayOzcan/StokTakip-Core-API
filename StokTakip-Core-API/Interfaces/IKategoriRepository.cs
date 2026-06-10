using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IKategoriRepository
    {
        Task<ICollection<Kategoriler>> GetKategoriler();
        Task<bool> KategoriEkle(Kategoriler kategori);
    }
}