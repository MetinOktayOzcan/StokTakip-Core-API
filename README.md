# Stok Takip Sistemi - Core API

Katmanlı mimari, DTO kullanımı, Repository Pattern, güvenlik ve Exception Handling pratikleri üzerine geliştirdiğim bir stok takip API projesidir. Temel stok işlemlerini (ürün giriş/çıkış, kategori yönetimi) gerçekleştirirken, kod tekrarını önleyen, test edilebilirliği artıran ve uçtan uca güvenli bir altyapı kurmayı hedefledim. API şu anda Azure üzerinde canlı olarak çalışmaktadır.

## Klasör Yapısı ve Mimari Tercihler

* **Controllers:** Gelen HTTP isteklerini karşılar. İçerisinde hiçbir veritabanı sorgusu barındırmaz, sadece yönlendirme yapar.
* **Models:** Veritabanındaki tabloların C# tarafındaki karşılıkları.
* **DTOs:** Dışarıya açılan ve dışarıdan alınan verileri filtrelediğim nesneler.
* **Validators:** Veri doğrulama işlemlerini DTO'lar üzerinden ayırarak daha esnek ve yönetilebilir kılmak için FluentValidation kullandığım katman.
* **Data:** Entity Framework Core için `DbContext` ayarları.
* **Interfaces & Repository:** Veritabanı işlemlerinin yapıldığı asıl katman. Controller'lar veritabanına doğrudan bağlanmak yerine, sadece Interface'ler üzerinden buradaki metodları çağırır. Bu sayede Controller katmanı veri erişim detaylarından bağımsız kaldı.
* **Services:** İş kurallarını ve kompleks işlemleri (Audit Log, stok hareketi hesaplamaları, zaman yönetimi) kontrol ettiğim katman.
* **Middlewares:** Global hata yönetimi ve güvenlik başlıkları için kullandığım ara katman. Beklenmedik bir kod çökmesi yaşandığında uygulamanın patlamasını engeller; hatayı arka planda maskeleyerek loglar ve arayüze standart ve temiz bir JSON hata mesajı fırlatır.

## Öne Çıkan Güvenlik ve Mimari Geliştirmeleri

Sistemi canlı ortama uygun hale getirmek için uyguladığım temel savunma mekanizmaları:

* **HttpOnly Cookie Tabanlı JWT:** Kimlik doğrulama token'larını istemci tarafında açığa çıkarmak yerine, XSS saldırılarına karşı güvenli çerezlerde taşıyorum.
* **Rate Limiting ve IP Spoofing Koruması:** IP ve kullanıcı bazlı kısıtlamalarla kaba kuvvet ve DoS saldırılarını engellerken, sahte IP manipülasyonlarına karşı ağ geçidi önlemleri aldım.
* **Güvenli Oturum Yönetimi:** Bir kullanıcının şifresi yenilendiğinde, veritabanındaki mevcut token'ları geçersiz kılarak diğer cihazlardaki oturumlarını anında sonlandıran bir yapı kurdum.
* **Race Condition Koruması:** Aynı anda aynı ürüne yapılan stok giriş/çıkış işlemlerinde veritabanı tutarsızlıklarını önlemek için işlem çakışmalarını yönetiyorum.
* **Gelişmiş Audit Logging:** Sistemi kullananların IP, cihaz bilgileri ve gerçekleştirdikleri kritik eylemleri veritabanına kaydediyorum.

## Kullanılan Teknolojiler

* .NET Core 10.0 (C#)
* Entity Framework Core & MSSQL
* Azure App Service & Environment Variables
* Serilog (Dosya bazlı güvenli loglama)
* FluentValidation (Veri doğrulaması)
* BCrypt.Net (Parola hashleme)
* Docker (Konteynerizasyon)
* xUnit & Moq (Birim Testleri)

---

## Kurulum ve Kullanım

Projeyi kendi ortamınızda çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

### 1. Gereksinimler
* .NET 10.0 SDK
* SQL Server (veya Docker üzerinde çalışan bir MSSQL imajı)

### 2. Projeyi Klonlayın
```bash
git clone https://github.com/kullaniciadin/stok-takip-api.git
cd stok-takip-api
```

### 3. Ortam Değişkenleri (Environment Variables) Konfigürasyonu
Güvenlik gereği hassas veriler `appsettings.json` içerisinde boş bırakılmıştır. 

**Lokal Kurulum İçin:**
Projeyi çalıştırmadan önce işletim sisteminizde veya `appsettings.Development.json` dosyasında şu değerleri tanımlayın:
* `ConnectionStrings:DefaultConnection` -> Kendi SQL Server bağlantı dizeniz.
* `JWT_SECRET_KEY` -> Token'ların imzalanması için kullanılacak en az 32 karakterlik gizli anahtar.
* `DefaultAdminPassword` -> Sistem ilk kez ayağa kalktığında oluşturulacak varsayılan admin hesabının şifresi.

**Azure (Canlı) Kurulum İçin:**
Proje Azure üzerinde çalışırken bu ayarları kod içerisine yazmak yerine, Azure portalında ilgili uygulamanın **Environment variables (Ortam değişkenleri)** sekmesine eklemelisiniz. ASP.NET Core bu değerleri hiyerarşik olarak (örneğin `ConnectionStrings__DefaultConnection` şeklinde çift alt çizgi ile) otomatik okuyacak şekilde yapılandırılmıştır.

### 4. Veritabanı Göçleri (Migrations)
Proje dizininde terminali açın ve Entity Framework Core kullanarak veritabanını oluşturun:
```bash
dotnet ef database update
```

### 5. Projeyi Çalıştırma
```bash
dotnet run
```
*Not: Uygulama ayağa kalktığında sistem yöneticisi eksikse otomatik olarak oluşturulacaktır.*

### 6. Test Etme ve İlk Giriş
* Uygulama çalıştıktan sonra `POST /api/Auth/login` uç noktasına kullanıcı adı `admin` ve belirlediğiniz `DefaultAdminPassword` ile istek atarak ilk girişinizi yapabilir ve güvenli çerezlerin tarayıcınıza oturduğunu görebilirsiniz.
* Klasör içerisindeki `.http` dosyalarını kullanarak Visual Studio üzerinden veya Postman gibi araçlarla API testlerini gerçekleştirebilirsiniz.
