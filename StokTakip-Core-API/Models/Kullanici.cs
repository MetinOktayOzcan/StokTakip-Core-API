using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Required]
        [MaxLength(50)]
        public required string KullaniciAdi { get; set; }

        [Required]
        [MaxLength(255)]
        public required string SifreHash { get; set; }

        [Required]
        [MaxLength(100)]
        public required string AdSoyad { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Rol { get; set; }

        [MaxLength(255)]
        public string? RefreshTokenHash { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
        public int HataliGirisSayisi { get; set; }
        public DateTime? KilitlenmeTarihi { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}