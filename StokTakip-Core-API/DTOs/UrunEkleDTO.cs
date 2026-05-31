using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class UrunEkleDTO
    {
        public int UrunID { get; set; }

        [Required(ErrorMessage = "Ürün adı boş geçilemez!")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ürün adı en az 2, en fazla 100 karakter olmalıdır.")]
        public required string UrunAdi { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal BirimFiyat { get; set; }

        [Range(0, 10000, ErrorMessage = "Stok miktarı 0 ile 10.000 arasında olmalıdır.")]
        public int StokMiktari { get; set; }

        public string? KategoriAdi { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen geçerli bir kategori ID giriniz.")]
        public int KategoriID { get; set; }
    }
}