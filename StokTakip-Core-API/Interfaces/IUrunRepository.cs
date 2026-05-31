using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IUrunRepository
    {
        Task<ICollection<urun>> GetUrunler();
        Task<bool> UrunEkle(urun urun);

        Task<urun> GetUrunById(int id);
        Task<bool> UrunGuncelle(urun urun);
        Task<bool> UrunSil(urun urun);
        Task<bool> UrununStokHareketiVarMi(int id);

        Task<bool> Save();
    }
}