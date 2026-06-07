using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StokTakip_Core_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly stokTakipContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(stokTakipContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(x => x.KullaniciAdi == loginModel.KullaniciAdi);

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(loginModel.Sifre, kullanici.SifreHash))
            {
                return Unauthorized(new { mesaj = "Kullanıcı adı veya şifre hatalı" });
            }

            var token = GenerateJwtToken(kullanici);
            return Ok(new { token = token });
        }
        /* Kullanıcı oluşturmak için bu metodu kullanabilirsiniz. Ancak, güvenlik nedeniyle bu metodu sadece ilk kurulumda çalıştırmanızı öneririm.
        [HttpPost("ilk-kurulum")]
        public async Task<IActionResult> IlkKurulum()
        {
            if (await _context.Kullanicilar.AnyAsync())
                return BadRequest("Sistemde zaten kullanıcı var. Bu metot sadece 1 kez çalışır.");

            var admin = new Kullanici
            {
                KullaniciAdi = "admin",
                SifreHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Rol = "Admin"
            };

            _context.Kullanicilar.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("İlk admin kullanıcısı başarıyla oluşturuldu.");
        }
        */

        private string GenerateJwtToken(Kullanici kullanici)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDTO
    {
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
    }
}