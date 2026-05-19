using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class IslemGecmisi
    {
        [Key]
        public int LogID { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string? Kullanici { get; set; }
        public string? IslemTipi { get; set; }
        public string? Detay { get; set; }
    }
}
