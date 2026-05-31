using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<stokTakipContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IKategoriRepository, StokTakip_Core_API.Repository.KategoriRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IUrunRepository, StokTakip_Core_API.Repository.UrunRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IStokHareketleriRepository, StokTakip_Core_API.Repository.StokHareketleriRepository>();
builder.Services.AddScoped<StokTakip_Core_API.Interfaces.IIslemGecmisiRepository, StokTakip_Core_API.Repository.IslemGecmisiRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
