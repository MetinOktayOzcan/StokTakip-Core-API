using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class urun
    {
        [Key]
        public string UrunAdi { get; set; }

        public int UrunID { get; set; }
        public int UrunId { get; internal set; }
        public int? KategoriID { get; set; }

        public virtual Kategoriler Kategori { get; set; }

        public int StokAdedi { get; set; }

        public decimal BirimFiyati { get; set; }

        public DateTime EklenmeTarihi { get; set; }
    }
}
