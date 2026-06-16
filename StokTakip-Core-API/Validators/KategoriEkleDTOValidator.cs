using FluentValidation;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Validators
{
    public class KategoriEkleDTOValidator : AbstractValidator<KategoriEkleDTO>
    {
        public KategoriEkleDTOValidator()
        {
            RuleFor(x => x.KategoriAdi)
                .NotEmpty().WithMessage("Kategori adı zorunludur.")
                .Length(2, 50).WithMessage("Kategori adı 2-50 karakter arası olmalıdır.")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s\-]+$")
                .WithMessage("Kategori adında zararlı karakter tespit edildi. Sadece harf, rakam ve tire (-) kullanılabilir.");
        }
    }
}