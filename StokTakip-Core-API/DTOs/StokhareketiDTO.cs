namespace StokTakip_Core_API.DTOs
{
    public class StokhareketiDTO
    {
        public int HareketID { get; set; }
        public int UrunID { get; set; }
        public string? UrunAdi { get; set; }
        public string? IslemTuru { get; set; }
        public int Miktar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string? Kullanici { get; set; }
        public string? Konum { get; set; }
        public string? Aciklama { get; set; }
    }
}