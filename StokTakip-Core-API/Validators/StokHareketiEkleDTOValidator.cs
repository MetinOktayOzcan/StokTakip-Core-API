using FluentValidation;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Validators
{
    public class StokHareketiEkleDTOValidator : AbstractValidator<StokHareketiEkleDTO>
    {
        public StokHareketiEkleDTOValidator()
        {
            RuleFor(x => x.UrunID)
                .GreaterThan(0).WithMessage("Geçerli bir ürün seçilmelidir.");

            RuleFor(x => x.Miktar)
                .InclusiveBetween(1, 100000).WithMessage("Miktar 1 ile 100.000 arasında olmalıdır.");

            RuleFor(x => x.Konum)
                .MaximumLength(50).WithMessage("Konum bilgisi en fazla 50 karakter olabilir.")
                .Matches(@"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s\-_.,]+$")
                .WithMessage("Konum alanında geçersiz karakterler kullanılamaz.")
                .When(x => !string.IsNullOrEmpty(x.Konum));

            RuleFor(x => x.Aciklama)
                .MaximumLength(255).WithMessage("Açıklama en fazla 255 karakter olabilir.")
                .Must(x => !x.Contains("<") && !x.Contains(">"))
                .WithMessage("Açıklama alanı HTML etiketleri içeremez.")
                .When(x => !string.IsNullOrEmpty(x.Aciklama));
        }
    }
}