using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string SifreHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Kullanici";
    }
}