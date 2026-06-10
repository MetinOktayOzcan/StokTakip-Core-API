using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StokTakip_Core_API.Migrations
{
    /// <inheritdoc />
    public partial class LogSistemiKuruldu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hayalet tabloyu silme çabalarını bypass ettik, direkt yeni tabloyu kuruyoruz.
            migrationBuilder.CreateTable(
                name: "IslemGecmisi",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IslemTipi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detay = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IslemGecmisi", x => x.LogID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IslemGecmisi",
                table: "IslemGecmisi");

            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "Kullanicilar");

            migrationBuilder.RenameTable(
                name: "IslemGecmisi",
                newName: "IslemGecmisi_Logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IslemGecmisi_Logs",
                table: "IslemGecmisi_Logs",
                column: "LogID");
        }
    }
}
