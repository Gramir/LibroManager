using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "AutorId", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel García Márquez" },
                    { 2, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mario Vargas Llosa" },
                    { 3, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isabel Allende" }
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "CategoriaId", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Novela" },
                    { 2, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Historia" },
                    { 3, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ciencia Ficción" }
                });

            migrationBuilder.InsertData(
                table: "Estudiantes",
                columns: new[] { "EstudianteId", "Email", "FechaCreacion", "FechaInscripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "juan.perez@ejemplo.com", new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Juan Pérez" },
                    { 2, "maria.garcia@ejemplo.com", new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "María García" }
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "LibroId", "AutorId", "CategoriaId", "Estado", "FechaCreacion", "ISBN", "NumeroEjemplares", "Titulo" },
                values: new object[,]
                {
                    { 1, 1, 1, 0, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780307474728", 3, "Cien años de soledad" },
                    { 2, 2, 1, 0, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "9788420471839", 2, "La ciudad y los perros" },
                    { 3, 3, 1, 0, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "9780525433477", 4, "La casa de los espíritus" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Estudiantes",
                keyColumn: "EstudianteId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Estudiantes",
                keyColumn: "EstudianteId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "AutorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "AutorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "AutorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValue: 1);
        }
    }
}
