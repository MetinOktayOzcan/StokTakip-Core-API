using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class StokHareketiEkleDTO
    {
        [Required]
        public int UrunID { get; set; }

        [Required(ErrorMessage = "İşlem türü belirtilmelidir.")]
        public required string IslemTuru { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Miktar 1 ile 100.000 arasında olmalıdır.")]
        public int Miktar { get; set; }

        public string? Konum { get; set; }

        public string? Aciklama { get; set; }
    }
}