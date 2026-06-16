namespace StokTakip_Core_API.DTOs
{
    public class KullaniciDTO
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public string? Sifre { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}