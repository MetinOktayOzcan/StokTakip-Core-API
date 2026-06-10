using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class Kategoriler
    {
        [Key]
        public int KategoriID { get; set; }
        public string? KategoriAdi { get; set; }
    }
}