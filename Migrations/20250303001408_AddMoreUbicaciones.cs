using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreUbicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ubicaciones",
                columns: new[] { "UbicacionId", "Estante", "Nivel", "Posicion" },
                values: new object[,]
                {
                    // Más posiciones para el estante A
                    { 16, "A", 1, 5 },
                    { 17, "A", 1, 6 },
                    { 18, "A", 2, 3 },
                    { 19, "A", 2, 4 },
                    { 20, "A", 3, 1 },
                    { 21, "A", 3, 2 },

                    // Más posiciones para el estante B
                    { 22, "B", 1, 3 },
                    { 23, "B", 1, 4 },
                    { 24, "B", 2, 4 },
                    { 25, "B", 3, 1 },
                    { 26, "B", 3, 2 },

                    // Más posiciones para el estante C
                    { 27, "C", 1, 3 },
                    { 28, "C", 1, 4 },
                    { 29, "C", 2, 1 },
                    { 30, "C", 2, 2 },

                    // Nuevo estante D
                    { 31, "D", 1, 1 },
                    { 32, "D", 1, 2 },
                    { 33, "D", 1, 3 },
                    { 34, "D", 1, 4 },
                    { 35, "D", 2, 1 },
                    { 36, "D", 2, 2 },
                    { 37, "D", 2, 3 },
                    { 38, "D", 3, 1 },
                    { 39, "D", 3, 2 },
                    { 40, "D", 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = 16; i <= 40; i++)
            {
                migrationBuilder.DeleteData(
                    table: "Ubicaciones",
                    keyColumn: "UbicacionId",
                    keyValue: i);
            }
        }
    }
}
