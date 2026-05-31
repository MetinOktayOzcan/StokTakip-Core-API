using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class KategoriEkleDTO
    {
        [Required(ErrorMessage = "Kategori adı boş geçilemez!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Kategori adı en az 2, en fazla 50 karakter olmalıdır.")]
        public required string KategoriAdi { get; set; }
    }
}