using StokTakip_Core_API.Models;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Interfaces
{
    public interface IStokHareketleriRepository
    {
        Task<ICollection<StokhareketiDTO>> GetStokHareketleri();
        Task<bool> StokHareketiEkle(StokHareketleri hareket);
        Task<bool> Save();
    }
}