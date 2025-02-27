using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNumeroEjemplares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libros_ISBN",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "NumeroEjemplares",
                table: "Libros");

            migrationBuilder.AddColumn<string>(
                name: "Serial",
                table: "Libros",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "Libros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 1,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "GAB-SOL-001", "Estante A1-01" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 2,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "ROW-HP1-001", "Estante B2-01" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 3,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "KIN-RES-001", "Estante C3-01" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 4,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "ALL-ESP-001", "Estante A1-02" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 5,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "MUR-TOK-001", "Estante A1-03" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 6,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "GAB-AMO-001", "Estante A1-04" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 7,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "ROW-HP2-001", "Estante B2-02" });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 8,
                columns: new[] { "Serial", "Ubicacion" },
                values: new object[] { "KIN-IT-001", "Estante C3-02" });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_ISBN",
                table: "Libros",
                column: "ISBN");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Serial",
                table: "Libros",
                column: "Serial",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libros_ISBN",
                table: "Libros");

            migrationBuilder.DropIndex(
                name: "IX_Libros_Serial",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "Serial",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Libros");

            migrationBuilder.AddColumn<int>(
                name: "NumeroEjemplares",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 1,
                column: "NumeroEjemplares",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 2,
                column: "NumeroEjemplares",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 3,
                column: "NumeroEjemplares",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 4,
                column: "NumeroEjemplares",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 5,
                column: "NumeroEjemplares",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 6,
                column: "NumeroEjemplares",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 7,
                column: "NumeroEjemplares",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 8,
                column: "NumeroEjemplares",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Libros_ISBN",
                table: "Libros",
                column: "ISBN",
                unique: true);
        }
    }
}
