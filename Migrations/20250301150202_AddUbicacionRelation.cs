using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUbicacionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Libros_ISBN",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Libros");

            migrationBuilder.AddColumn<int>(
                name: "UbicacionId",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ubicaciones",
                columns: table => new
                {
                    UbicacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Posicion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ubicaciones", x => x.UbicacionId);
                });

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 1,
                column: "UbicacionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 2,
                column: "UbicacionId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 3,
                column: "UbicacionId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 4,
                column: "UbicacionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 5,
                column: "UbicacionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 6,
                column: "UbicacionId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 7,
                column: "UbicacionId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 8,
                column: "UbicacionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 9,
                column: "UbicacionId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 10,
                column: "UbicacionId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 11,
                column: "UbicacionId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 12,
                column: "UbicacionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 13,
                column: "UbicacionId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 14,
                column: "UbicacionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 15,
                column: "UbicacionId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 16,
                column: "UbicacionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 17,
                column: "UbicacionId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 18,
                column: "UbicacionId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 19,
                column: "UbicacionId",
                value: 9);

            migrationBuilder.InsertData(
                table: "Ubicaciones",
                columns: new[] { "UbicacionId", "Estante", "Nivel", "Posicion" },
                values: new object[,]
                {
                    { 1, "A", 1, 1 },
                    { 2, "A", 1, 2 },
                    { 3, "A", 1, 3 },
                    { 4, "A", 1, 4 },
                    { 5, "B", 2, 1 },
                    { 6, "B", 2, 2 },
                    { 7, "B", 2, 3 },
                    { 8, "C", 3, 1 },
                    { 9, "C", 3, 2 },
                    { 10, "A", 2, 1 },
                    { 11, "A", 2, 2 },
                    { 12, "B", 1, 1 },
                    { 13, "B", 1, 2 },
                    { 14, "C", 1, 1 },
                    { 15, "C", 1, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_UbicacionId",
                table: "Libros",
                column: "UbicacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Libros_Ubicaciones_UbicacionId",
                table: "Libros",
                column: "UbicacionId",
                principalTable: "Ubicaciones",
                principalColumn: "UbicacionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Libros_Ubicaciones_UbicacionId",
                table: "Libros");

            migrationBuilder.DropTable(
                name: "Ubicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Libros_UbicacionId",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "UbicacionId",
                table: "Libros");

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
                column: "Ubicacion",
                value: "Estante A1-01");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 2,
                column: "Ubicacion",
                value: "Estante B2-01");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 3,
                column: "Ubicacion",
                value: "Estante C3-01");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 4,
                column: "Ubicacion",
                value: "Estante A1-02");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 5,
                column: "Ubicacion",
                value: "Estante A1-03");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 6,
                column: "Ubicacion",
                value: "Estante A1-04");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 7,
                column: "Ubicacion",
                value: "Estante B2-02");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 8,
                column: "Ubicacion",
                value: "Estante C3-02");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 9,
                column: "Ubicacion",
                value: "Estante A1-05");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 10,
                column: "Ubicacion",
                value: "Estante A1-06");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 11,
                column: "Ubicacion",
                value: "Estante B2-03");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 12,
                column: "Ubicacion",
                value: "Estante B2-04");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 13,
                column: "Ubicacion",
                value: "Estante B2-05");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 14,
                column: "Ubicacion",
                value: "Estante C3-03");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 15,
                column: "Ubicacion",
                value: "Estante A1-07");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 16,
                column: "Ubicacion",
                value: "Estante A1-08");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 17,
                column: "Ubicacion",
                value: "Estante B2-06");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 18,
                column: "Ubicacion",
                value: "Estante B2-07");

            migrationBuilder.UpdateData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 19,
                column: "Ubicacion",
                value: "Estante C3-04");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_ISBN",
                table: "Libros",
                column: "ISBN");
        }
    }
}
