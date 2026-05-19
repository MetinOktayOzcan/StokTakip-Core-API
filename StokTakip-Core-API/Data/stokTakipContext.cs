using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Data
{
    public class stokTakipContext : DbContext    
    {
        public stokTakipContext(DbContextOptions<stokTakipContext> options) : base(options)
        {
        }
        public DbSet<urun> Urunler { get; set; }
        public DbSet<Kategoriler> Kategoriler { get; set; }
        public DbSet<IslemGecmisi> IslemGecmisi_Logs { get; set; }
        public DbSet<StokHareketleri> StokHareketleri { get; set; }
    }
}
