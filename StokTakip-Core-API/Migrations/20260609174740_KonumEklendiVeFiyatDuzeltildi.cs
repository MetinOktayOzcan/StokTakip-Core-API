using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StokTakip_Core_API.Migrations
{
    /// <inheritdoc />
    public partial class KonumEklendiVeFiyatDuzeltildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "StokHareketleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konum",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "Konum",
                table: "StokHareketleri");
        }
    }
}
