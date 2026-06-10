using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }
        public required string KullaniciAdi { get; set; }
        public required string SifreHash { get; set; }
        public required string Rol { get; set; }
        public string? AdSoyad { get; set; }
    }
}