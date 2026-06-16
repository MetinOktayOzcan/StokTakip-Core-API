using FluentValidation;
using StokTakip_Core_API.DTOs;

namespace StokTakip_Core_API.Validators
{
    public class KullaniciDTOValidator : AbstractValidator<KullaniciDTO>
    {
        public KullaniciDTOValidator()
        {
            RuleFor(x => x.KullaniciAdi)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");

            RuleFor(x => x.Sifre)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(10).WithMessage("Şifre en az 10 karakter olmalıdır.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{10,}$")
                .WithMessage("Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")
                .When(x => !string.IsNullOrEmpty(x.Sifre));

            RuleFor(x => x.AdSoyad)
                .NotEmpty().WithMessage("Ad Soyad zorunludur.")
                .Length(2, 100).WithMessage("Lütfen geçerli bir Ad Soyad giriniz.")
                .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Ad Soyad sadece harflerden oluşmalıdır.");

            RuleFor(x => x.Rol)
                .NotEmpty().WithMessage("Rol alanı boş bırakılamaz.")
                .Must(rol => rol == "Admin" || rol == "Yonetici" || rol == "Personel")
                .WithMessage("Geçersiz rol türü belirttiniz. Sadece 'Admin', 'Yonetici' veya 'Personel' girilebilir.");
        }
    }
}