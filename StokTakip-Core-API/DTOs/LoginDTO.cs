using System.ComponentModel.DataAnnotations;

namespace StokTakip_Core_API.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Sifre { get; set; } = string.Empty;
    }

    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "Refresh token boş olamaz.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}