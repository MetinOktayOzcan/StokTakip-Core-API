using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IStokHareketleriRepository
    {
        Task<ICollection<StokHareketleri>> GetStokHareketleri();
        Task<bool> StokHareketiEkle(StokHareketleri hareket);


        Task<bool> Save();
    }
}