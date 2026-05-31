using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Interfaces
{
    public interface IIslemGecmisiRepository
    {
        Task<ICollection<IslemGecmisi>> GetIslemGecmisleri();
    }
}