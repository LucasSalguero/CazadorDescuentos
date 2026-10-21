using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CazadorDescuentos.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaveUnica = table.Column<string>(type: "TEXT", nullable: false),
                    Marca = table.Column<string>(type: "TEXT", nullable: false),
                    Producto = table.Column<string>(type: "TEXT", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", nullable: true),
                    Restricciones = table.Column<string>(type: "TEXT", nullable: true),
                    EnlaceComercio = table.Column<string>(type: "TEXT", nullable: false),
                    PrecioOriginal = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrecioPromocional = table.Column<decimal>(type: "TEXT", nullable: false),
                    PorcentajeDescuento = table.Column<decimal>(type: "TEXT", nullable: false),
                    VencimientoUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimaActualizacionUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OfertaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    Funciono = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_ClaveUnica",
                table: "Ofertas",
                column: "ClaveUnica",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_OfertaId_DeviceId",
                table: "Votos",
                columns: new[] { "OfertaId", "DeviceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "Votos");
        }
    }
}
