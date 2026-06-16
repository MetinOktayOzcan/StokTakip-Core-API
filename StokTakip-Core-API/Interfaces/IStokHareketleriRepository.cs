using StokTakip_Core_API.Models;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Interfaces
{
    public interface IStokHareketleriRepository
    {
        Task<ICollection<StokhareketiDTO>> GetStokHareketleri(int sayfa = 1, int boyut = 50);
        Task<bool> StokHareketiEkle(StokHareketleri hareket);
    }
}