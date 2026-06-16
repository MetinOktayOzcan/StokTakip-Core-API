namespace StokTakip_Core_API.Services
{
    public interface IAuditLogService
    {
        Task LogOlusturAsync(string islemTipi, string detay);
    }
}