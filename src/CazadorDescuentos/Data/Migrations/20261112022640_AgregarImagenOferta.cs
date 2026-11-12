using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CazadorDescuentos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenOferta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Ofertas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Ofertas");
        }
    }
}
