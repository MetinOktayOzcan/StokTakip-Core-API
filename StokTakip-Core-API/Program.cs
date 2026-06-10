using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StokTakip_Core_API.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<stokTakipContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("TumPlatformlar", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IKategoriRepository, StokTakip_Core_API.Repository.KategoriRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IUrunRepository, StokTakip_Core_API.Repository.UrunRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IStokHareketleriRepository, StokTakip_Core_API.Repository.StokHareketleriRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IIslemGecmisiRepository, StokTakip_Core_API.Repository.IslemGecmisiRepository>();

var app = builder.Build();

app.UseCors("TumPlatformlar");
app.UseHttpsRedirection();
app.UseMiddleware<StokTakip_Core_API.Middlewares.ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();