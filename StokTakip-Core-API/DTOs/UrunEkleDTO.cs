namespace StokTakip_Core_API.DTOs
{
    public class UrunEkleDTO
    {
        public string UrunAdi { get; set; } = string.Empty;
        public int KategoriID { get; set; }
        public decimal BirimFiyati { get; set; }
        public int StokAdedi { get; set; }
        public string? Konum { get; set; }
    }

    public class UrunGuncelleDTO
    {
        public string UrunAdi { get; set; } = string.Empty;
        public int KategoriID { get; set; }
        public decimal BirimFiyati { get; set; }
        public string? Konum { get; set; }
    }
}