using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public AuthController(IKullaniciRepository kullaniciRepository, IConfiguration configuration, IDistributedCache cache)
        {
            _kullaniciRepository = kullaniciRepository;
            _configuration = configuration;
            _cache = cache;
        }

        private void SetTokenCookies(string token, string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var expireMinutes = Convert.ToDouble(jwtSettings["ExpireMinutes"] ?? "30");

            bool isSecure = Request.IsHttps || Request.Headers["X-Forwarded-Proto"] == "https";

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isSecure,
                SameSite = isSecure ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isSecure,
                SameSite = isSecure ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("X-Access-Token", token, cookieOptions);
            Response.Cookies.Append("X-Refresh-Token", refreshToken, refreshCookieOptions);
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var kullanici = await _kullaniciRepository.GetKullaniciByKullaniciAdi(dto.KullaniciAdi);
            bool isPasswordValid = false;
            string dummySalt = "$2a$11$1234567890123456789012";

            if (kullanici != null)
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.SifreHash);
            }
            else
            {
                BCrypt.Net.BCrypt.HashPassword(dto.Sifre, dummySalt);
            }

            if (kullanici != null && kullanici.KilitlenmeTarihi.HasValue && kullanici.KilitlenmeTarihi.Value > DateTime.UtcNow)
            {
                var kalanDakika = Math.Ceiling((kullanici.KilitlenmeTarihi.Value - DateTime.UtcNow).TotalMinutes);
                return Unauthorized(new { Mesaj = $"Hesabınız kilitlendi. Lütfen {kalanDakika} dakika sonra tekrar deneyin." });
            }

            if (kullanici == null || !isPasswordValid)
            {
                if (kullanici != null)
                {
                    try
                    {
                        kullanici.HataliGirisSayisi += 1;

                        if (kullanici.HataliGirisSayisi >= 5)
                        {
                            kullanici.KilitlenmeTarihi = DateTime.UtcNow.AddMinutes(15);
                        }

                        await _kullaniciRepository.KullaniciGuncelle(kullanici);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        await Task.Delay(2000);
                    }
                }

                return Unauthorized(new { Mesaj = "Kullanıcı adı veya şifre hatalı." });
            }

            kullanici.HataliGirisSayisi = 0;
            kullanici.KilitlenmeTarihi = null;

            var token = GenerateJwtToken(kullanici);
            var refreshToken = GenerateRefreshToken();

            kullanici.RefreshTokenHash = HashToken(refreshToken);
            kullanici.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _kullaniciRepository.KullaniciGuncelle(kullanici);
            SetTokenCookies(token, refreshToken);

            return Ok(new
            {
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol
            });
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["X-Refresh-Token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { Mesaj = "Oturum süresi dolmuş veya geçersiz token." });

            var hashedToken = HashToken(refreshToken);
            var kullanici = await _kullaniciRepository.GetKullaniciByRefreshTokenHash(hashedToken);

            if (kullanici == null || kullanici.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized(new { Mesaj = "Oturum süresi dolmuş veya geçersiz token." });

            var newToken = GenerateJwtToken(kullanici);
            var newRefreshToken = GenerateRefreshToken();

            kullanici.RefreshTokenHash = HashToken(newRefreshToken);
            kullanici.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _kullaniciRepository.KullaniciGuncelle(kullanici);
            SetTokenCookies(newToken, newRefreshToken);

            return Ok();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return BadRequest(new { Mesaj = "Geçersiz oturum." });

            if (!string.IsNullOrEmpty(jti))
            {
                var jwtSettings = _configuration.GetSection("Jwt");
                var expireMinutes = Convert.ToDouble(jwtSettings["ExpireMinutes"] ?? "30");

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expireMinutes)
                };

                await _cache.SetStringAsync($"blacklist_{jti}", "true", cacheOptions);
            }

            var kullanici = await _kullaniciRepository.GetKullaniciById(userId);
            if (kullanici != null)
            {
                kullanici.RefreshTokenHash = null;
                kullanici.RefreshTokenExpiryTime = null;
                await _kullaniciRepository.KullaniciGuncelle(kullanici);
            }

            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");

            return Ok(new { Mesaj = "Başarıyla çıkış yapıldı ve oturum sonlandırıldı." });
        }

        private string GenerateJwtToken(Models.Kullanici kullanici)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = _configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT Key eksik.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciID.ToString()),
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol),
                new Claim("AdSoyad", kullanici.AdSoyad),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "",
                audience: jwtSettings["Audience"] ?? "",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"] ?? "30")),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string HashToken(string token)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
    }
}