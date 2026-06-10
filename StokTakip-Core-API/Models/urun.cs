using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokTakip_Core_API.Models
{
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }
        public required string UrunAdi { get; set; }
        public int? KategoriID { get; set; }
        public virtual Kategoriler? Kategori { get; set; }
        public int StokAdedi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyati { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public string? Konum { get; set; }
    }
}