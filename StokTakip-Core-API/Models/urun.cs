namespace StokTakip_Core_API.Models
{
    public class urun
    {
        public string UrunAdi { get; set; }

        public int UrunID { get; set; }

        public int? KategoriID { get; set; }

        public int StokAdedi { get; set; }

        public decimal BirimFiyati { get; set; }

        public DateTime EklenmeTarihi { get; set; }
    }
}
