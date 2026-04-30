using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorIA.Migrations
{
    /// <inheritdoc />
    public partial class TablaPersonas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Salario = table.Column<decimal>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "Activo", "Email", "Nombre", "Salario" },
                values: new object[,]
                {
                    { 1, true, "felipe.gavilan@example.com", "Felipe Gavilán", 45000m },
                    { 2, true, "maria.lopez@example.com", "María López", 52000m },
                    { 3, false, "carlos.rodriguez@example.com", "Carlos Rodríguez", 61000m },
                    { 4, false, "ana.martinez@example.com", "Ana Martínez", 48000m },
                    { 5, true, "luis.gomez@example.com", "Luis Gómez", 55000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
