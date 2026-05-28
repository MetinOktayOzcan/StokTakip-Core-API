using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class urun
    {
        [Key]
        public int UrunId { get; internal set; }

        public string UrunAdi { get; set; }
        public int? KategoriID { get; set; }


        public virtual Kategoriler Kategori { get; set; }

        public int StokAdedi { get; set; }

        public decimal BirimFiyati { get; set; }

        public DateTime EklenmeTarihi { get; set; }
    }
}
