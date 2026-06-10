using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class StokhareketiDTO
    {
        public int? HareketID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli bir ürün ID giriniz.")]
        public int UrunID { get; set; }

        [Required(ErrorMessage = "İşlem türü boş geçilemez!")]
        [RegularExpression("^(?i)(giriş|giris|çıkış|cikis)$", ErrorMessage = "İşlem türü sadece Giriş yada Çıkış olabilir.")]
        public required string IslemTuru { get; set; }

        [Range(1, 100001, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Miktar { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Aciklama { get; set; }

        public string? UrunAdi { get; set; }

        public DateTime? IslemTarihi { get; set; }
        public string? Konum { get; set; }
    }
}