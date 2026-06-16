using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Data
{
    public class stokTakipContext : DbContext
    {
        public stokTakipContext(DbContextOptions<stokTakipContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
        public DbSet<Urun> Urunler => Set<Urun>();
        public DbSet<Kategoriler> Kategoriler => Set<Kategoriler>();
        public DbSet<StokHareketleri> StokHareketleri => Set<StokHareketleri>();
        public DbSet<IslemGecmisi> IslemGecmisi => Set<IslemGecmisi>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<Urun>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Urun>()
                .Property(u => u.BirimFiyati)
                .HasPrecision(18, 2);
        }
    }
}