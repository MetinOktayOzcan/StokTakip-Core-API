namespace StokTakip_Core_API.DTOs
{
    public class UrunEkleDTO
    {
        public required string UrunAdi { get; set; }
        public decimal BirimFiyat { get; set; }
        public int StokMiktari { get; set; }
        public string? KategoriAdi { get; set; }
        public int UrunID { get; set; }
        public int KategoriID { get; set; }

    }
}
