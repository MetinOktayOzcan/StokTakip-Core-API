using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StokTakip_Core_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Kullanicilar",
                newName: "RefreshTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "Kullanicilar",
                newName: "RefreshToken");
        }
    }
}
