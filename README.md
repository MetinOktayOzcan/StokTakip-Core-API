# Stok Takip Sistemi - Core API

Katmanlı mimari, DTO kullanımı, Repository Pattern ve Exception Handling pratikleri üzerine geliştirdiğim bir stok takip API projesidir. Temel stok işlemlerini (ürün giriş/çıkış, kategori yönetimi) gerçekleştirirken, kod tekrarını önleyen ve test edilebilirliği artıran bir altyapı kurmayı hedefledim.

## Klasör Yapısı ve Mimari Tercihler

* **Controllers:** Gelen HTTP isteklerini karşılar. İçerisinde hiçbir veritabanı sorgusu (LINQ, DbContext vb.) barındırmaz, sadece yönlendirme yapar.
* **Models:** Veritabanındaki tabloların (Entity) C# tarafındaki karşılıkları.
* **DTOs:** Dışarıya açılan ve dışarıdan alınan verileri filtrelediğim nesneler. Ayrıca Data Annotations ile veri doğrulama (Validation) kurallarını da direkt bu nesnelerin üzerinde yönetiyorum.
* **Data:** Entity Framework Core için `DbContext` ayarları.
* **Interfaces & Repository:** Veritabanı (CRUD) işlemlerinin yapıldığı asıl katman. Controller'lar veritabanına doğrudan bağlanmak yerine, sadece Interface'ler üzerinden buradaki metodları çağırır. Bu sayede Controller katmanı veri erişim detaylarından bağımsız kaldı.
* **Middlewares:** Global hata yönetimi için kullandığım middleware katmanı. Beklenmedik bir kod çökmesi yaşandığında uygulamanın patlamasını engeller; hatayı arka planda yakalayıp arayüze standart ve temiz bir JSON hata mesajı fırlatır.

## Kullanılan Teknolojiler
* .NET Core (C#)
* Entity Framework Core
* MSSQL
* API Testleri: Visual Studio `.http` dosyaları

## To-Do Listesi
* [ ] JWT (JSON Web Token) kullanılarak endpoint yetkilendirme ve güvenlik işlemlerinin yapılması.
* [ ] Serilog entegrasyonu ile gelişmiş loglama.
* [ ] Listeleme operasyonları için sayfalama (Pagination) eklenmesi.