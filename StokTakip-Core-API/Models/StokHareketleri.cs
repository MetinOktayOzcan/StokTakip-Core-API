using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class StokHareketleri
    {
        [Key]
        public int HareketID { get; set; }
        public int UrunID { get; set; }
        public string? IslemTuru { get; set; }
        public int Miktar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string? Aciklama { get; set; }
        public string? Konum { get; set; }
    }
}