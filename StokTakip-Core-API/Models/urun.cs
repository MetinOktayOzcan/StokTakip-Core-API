using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StokTakip_Core_API.Models
{
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string UrunAdi { get; set; }

        public int? KategoriID { get; set; }
        public virtual Kategoriler? Kategori { get; set; }
        public int StokAdedi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyati { get; set; }
        public DateTime EklenmeTarihi { get; set; }

        [MaxLength(50)]
        public string? Konum { get; set; }

        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}