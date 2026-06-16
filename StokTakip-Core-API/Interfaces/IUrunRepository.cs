using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IUrunRepository
    {
        Task<ICollection<Urun>> GetUrunler(int sayfa = 1, int boyut = 50);
        Task<bool> UrunEkle(Urun urun);
        Task<Urun?> GetUrunById(int id);
        Task<bool> UrunGuncelle(Urun urun);
        Task<bool> UrunSil(Urun urun);
        Task<bool> UrununStokHareketiVarMi(int id);
    }
}