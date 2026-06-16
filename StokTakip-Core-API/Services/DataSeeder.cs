using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Services
{
    public static class DataSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<stokTakipContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            context.Database.Migrate();

            if (!context.Kullanicilar.Any())
            {
                var defaultPassword = configuration["DefaultAdminPassword"];

                if (string.IsNullOrWhiteSpace(defaultPassword))
                {
                    throw new InvalidOperationException("GÜVENLİK HATASI: 'DefaultAdminPassword' ortam değişkeni bulunamadı! Uygulama varsayılan şifre ile başlatılamaz.");
                }

                context.Kullanicilar.Add(new Kullanici
                {
                    KullaniciAdi = "admin",
                    AdSoyad = "Sistem Yöneticisi",
                    Rol = "Admin",
                    SifreHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                    RefreshTokenHash = null,
                    RefreshTokenExpiryTime = null
                });

                context.SaveChanges();
            }
        }
    }
}