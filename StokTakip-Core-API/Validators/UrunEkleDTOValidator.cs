using FluentValidation;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Validators
{
    public class UrunEkleDTOValidator : AbstractValidator<UrunEkleDTO>
    {
        public UrunEkleDTOValidator()
        {
            RuleFor(x => x.UrunAdi)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .Length(2, 100).WithMessage("Ürün adı 2-100 karakter arası olmalıdır.")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s\-_.,()]+$")
                .WithMessage("Ürün adında zararlı karakter tespit edildi. Sadece harf, rakam ve temel noktalama işaretleri kullanılabilir.");

            RuleFor(x => x.KategoriID)
                .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");

            RuleFor(x => x.BirimFiyati)
                .GreaterThanOrEqualTo(0).WithMessage("Birim fiyatı sıfırdan küçük olamaz.");

            RuleFor(x => x.StokAdedi)
                .GreaterThanOrEqualTo(0).WithMessage("Stok adedi eksi bir değer alamaz.");
        }
    }

    public class UrunGuncelleDTOValidator : AbstractValidator<UrunGuncelleDTO>
    {
        public UrunGuncelleDTOValidator()
        {
            RuleFor(x => x.UrunAdi)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .Length(2, 100).WithMessage("Ürün adı 2-100 karakter arası olmalıdır.")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s\-_.,()]+$")
                .WithMessage("Ürün adında zararlı karakter tespit edildi. Sadece harf, rakam ve temel noktalama işaretleri kullanılabilir.");

            RuleFor(x => x.KategoriID)
                .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");

            RuleFor(x => x.BirimFiyati)
                .GreaterThanOrEqualTo(0).WithMessage("Birim fiyatı sıfırdan küçük olamaz.");
        }
    }
}