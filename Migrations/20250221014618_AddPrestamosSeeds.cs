using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class AddPrestamosSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Prestamos",
                columns: new[] { "PrestamoId", "Estado", "EstudianteId", "FechaCreacion", "FechaDevolucion", "FechaPrestamo", "FechaVencimiento", "LibroId" },
                values: new object[,]
                {
                    { 1, 0, 1, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 0, 2, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 2, 1, new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, 1, 2, new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Prestamos",
                keyColumn: "PrestamoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Prestamos",
                keyColumn: "PrestamoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Prestamos",
                keyColumn: "PrestamoId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Prestamos",
                keyColumn: "PrestamoId",
                keyValue: 4);
        }
    }
}
