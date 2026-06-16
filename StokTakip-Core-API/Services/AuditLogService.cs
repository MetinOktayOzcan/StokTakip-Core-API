using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly stokTakipContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuditLogService(
            stokTakipContext context,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task LogOlusturAsync(string islemTipi, string detay)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            var islemYapanKullanici = user?.FindFirst("AdSoyad")?.Value
                                      ?? user?.Identity?.Name
                                      ?? "Sistem";

            var ipAdresi = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Bilinmeyen IP";
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "Bilinmeyen Cihaz";

            var zenginlestirilmisDetay = $"{detay} | IP: {ipAdresi} | Cihaz: {userAgent}";

            var log = new IslemGecmisi
            {
                IslemTarihi = _dateTimeProvider.Now,
                Kullanici = islemYapanKullanici,
                IslemTipi = islemTipi,
                Detay = zenginlestirilmisDetay
            };

            _context.IslemGecmisi.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}