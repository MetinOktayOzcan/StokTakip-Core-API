using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class KategoriEkleDTO
    {
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        public required string KategoriAdi { get; set; }
    }
}