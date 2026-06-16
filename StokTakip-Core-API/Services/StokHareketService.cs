using Microsoft.EntityFrameworkCore;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.DTOs;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;

namespace StokTakip_Core_API.Services
{
    public class StokHareketService : IStokHareketService
    {
        private readonly IUrunRepository _urunRepository;
        private readonly IStokHareketleriRepository _stokRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly stokTakipContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public StokHareketService(
            IUrunRepository urunRepository,
            IStokHareketleriRepository stokRepository,
            IAuditLogService auditLogService,
            stokTakipContext context,
            IDateTimeProvider dateTimeProvider)
        {
            _urunRepository = urunRepository;
            _stokRepository = stokRepository;
            _auditLogService = auditLogService;
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(bool BasariliMi, string Mesaj)> StokHareketiIsleAsync(StokHareketiEkleDTO dto)
        {
            var urun = await _urunRepository.GetUrunById(dto.UrunID);
            if (urun == null)
                return (false, "Ürün bulunamadı.");

            if (!Enum.TryParse<Enums.IslemTuru>(dto.IslemTuru, true, out var islemTuru))
                return (false, "Geçersiz işlem türü. (Giris, Cikis veya SayimDuzeltmesi olmalıdır.)");

            if (islemTuru == Enums.IslemTuru.Cikis && urun.StokAdedi < dto.Miktar)
                return (false, $"Yetersiz stok. (Mevcut: {urun.StokAdedi})");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                checked
                {
                    switch (islemTuru)
                    {
                        case Enums.IslemTuru.Giris:
                            urun.StokAdedi += dto.Miktar;
                            break;
                        case Enums.IslemTuru.Cikis:
                            urun.StokAdedi -= dto.Miktar;
                            break;
                        case Enums.IslemTuru.SayimDuzeltmesi:
                            urun.StokAdedi = dto.Miktar;
                            break;
                    }
                }

                var hareket = new StokHareketleri
                {
                    UrunID = dto.UrunID,
                    IslemTuru = islemTuru.ToString(),
                    Miktar = dto.Miktar,
                    Konum = dto.Konum,
                    Aciklama = dto.Aciklama,
                    IslemTarihi = _dateTimeProvider.Now
                };

                await _urunRepository.UrunGuncelle(urun);
                await _stokRepository.StokHareketiEkle(hareket);
                await _auditLogService.LogOlusturAsync("Stok Hareketi", $"'{urun.UrunAdi}' için {dto.Miktar} adet {hareket.IslemTuru} yapıldı.");

                await transaction.CommitAsync();
                return (true, "Stok işlemi başarıyla tamamlandı.");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return (false, "Sistem meşgul, ürün stoku başka bir kullanıcı tarafından güncellendi. Lütfen tekrar deneyin.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}