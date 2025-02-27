using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleBookCopies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "LibroId", "AutorId", "CategoriaId", "Estado", "FechaCreacion", "ISBN", "Serial", "Titulo", "Ubicacion" },
                values: new object[,]
                {
                    { 9, 1, 1, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780307474728", "GAB-SOL-002", "Cien años de soledad", "Estante A1-05" },
                    { 10, 1, 1, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780307474728", "GAB-SOL-003", "Cien años de soledad", "Estante A1-06" },
                    { 11, 2, 2, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788478884452", "ROW-HP1-002", "Harry Potter y la piedra filosofal", "Estante B2-03" },
                    { 12, 2, 2, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788478884452", "ROW-HP1-003", "Harry Potter y la piedra filosofal", "Estante B2-04" },
                    { 13, 2, 2, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788478884452", "ROW-HP1-004", "Harry Potter y la piedra filosofal", "Estante B2-05" },
                    { 14, 3, 3, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788497593793", "KIN-RES-002", "El resplandor", "Estante C3-03" },
                    { 15, 4, 1, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780525433477", "ALL-ESP-002", "La casa de los espíritus", "Estante A1-07" },
                    { 16, 4, 1, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780525433477", "ALL-ESP-003", "La casa de los espíritus", "Estante A1-08" },
                    { 17, 2, 2, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788478884957", "ROW-HP2-002", "Harry Potter y la cámara secreta", "Estante B2-06" },
                    { 18, 2, 2, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788478884957", "ROW-HP2-003", "Harry Potter y la cámara secreta", "Estante B2-07" },
                    { 19, 3, 3, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9781501142970", "KIN-IT-002", "It", "Estante C3-04" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 19);
        }
    }
}
