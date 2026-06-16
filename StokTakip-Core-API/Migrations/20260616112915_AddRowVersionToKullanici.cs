using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StokTakip_Core_API.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToKullanici : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Kullanicilar",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Kullanicilar");
        }
    }
}
