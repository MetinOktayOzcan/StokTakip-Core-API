using Microsoft.AspNetCore.Mvc;
using Moq;
using StokTakip_Core_API.Controllers;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Models;
using StokTakip_Core_API.Services;
using Xunit;

namespace StokTakip_Core_API.Tests
{
    public class UrunlerControllerTests
    {
        private readonly Mock<IUrunRepository> _mockUrunRepo;
        private readonly Mock<IAuditLogService> _mockAuditLog;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Mock<IKategoriRepository> _mockKategoriRepo;
        private readonly UrunlerController _controller;

        public UrunlerControllerTests()
        {
            _mockUrunRepo = new Mock<IUrunRepository>();
            _mockAuditLog = new Mock<IAuditLogService>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockKategoriRepo = new Mock<IKategoriRepository>();

            _controller = new UrunlerController(
                _mockUrunRepo.Object,
                _mockAuditLog.Object,
                _mockDateTime.Object,
                _mockKategoriRepo.Object);
        }

        [Fact]
        public async Task UrunSil_StokSifirdanBuyukse_BadRequestDoner()
        {
            var testUrun = new Urun
            {
                UrunId = 1,
                UrunAdi = "Test Ürünü",
                StokAdedi = 5,
                BirimFiyati = 100,
                IsDeleted = false
            };

            _mockUrunRepo.Setup(r => r.GetUrunById(1)).ReturnsAsync(testUrun);

            var result = await _controller.UrunSil(1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Silmeden önce stok sıfırlanmalıdır", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task UrunSil_UrunBulunamazsa_NotFoundDoner()
        {
            _mockUrunRepo.Setup(r => r.GetUrunById(99)).ReturnsAsync((Urun?)null);

            var result = await _controller.UrunSil(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UrunSil_BasariliIse_IsDeletedTrueYapar_Ve_NoContentDoner()
        {
            var testUrun = new Urun
            {
                UrunId = 1,
                UrunAdi = "Test Ürünü",
                StokAdedi = 0,
                BirimFiyati = 100,
                IsDeleted = false
            };

            _mockUrunRepo.Setup(r => r.GetUrunById(1)).ReturnsAsync(testUrun);
            _mockUrunRepo.Setup(r => r.UrunGuncelle(It.IsAny<Urun>())).ReturnsAsync(true);

            var result = await _controller.UrunSil(1);

            Assert.True(testUrun.IsDeleted);
            Assert.IsType<NoContentResult>(result);
        }
    }
}