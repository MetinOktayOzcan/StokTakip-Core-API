using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StokTakip_Core_API.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciTablosunuEkleSon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanici_StokHareketleri_StokHareketleriHareketID",
                table: "Kullanici");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici");

            migrationBuilder.DropIndex(
                name: "IX_Kullanici_StokHareketleriHareketID",
                table: "Kullanici");

            migrationBuilder.DropColumn(
                name: "StokHareketleriHareketID",
                table: "Kullanici");

            migrationBuilder.RenameTable(
                name: "Kullanici",
                newName: "Kullanicilar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar");

            migrationBuilder.RenameTable(
                name: "Kullanicilar",
                newName: "Kullanici");

            migrationBuilder.AddColumn<int>(
                name: "StokHareketleriHareketID",
                table: "Kullanici",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanici_StokHareketleriHareketID",
                table: "Kullanici",
                column: "StokHareketleriHareketID");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanici_StokHareketleri_StokHareketleriHareketID",
                table: "Kullanici",
                column: "StokHareketleriHareketID",
                principalTable: "StokHareketleri",
                principalColumn: "HareketID");
        }
    }
}
