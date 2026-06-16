using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Interfaces
{
    public interface IStokHareketService
    {
        Task<(bool BasariliMi, string Mesaj)> StokHareketiIsleAsync(StokHareketiEkleDTO dto);
    }
}