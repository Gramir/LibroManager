using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibroManager.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Datos semilla para Categorías
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "CategoriaId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Ciencia Ficción" },
                    { 2, "Historia" },
                    { 3, "Programación" },
                    { 4, "Literatura" },
                    { 5, "Matemáticas" },
                    { 6, "Psicología" },
                    { 7, "Arte" },
                    { 8, "Medicina" },
                    { 9, "Economía" },
                    { 10, "Idiomas" }
                });

            // Datos semilla para Autores
            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "AutorId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Isaac Asimov" },
                    { 2, "Gabriel García Márquez" },
                    { 3, "Robert Martin" },
                    { 4, "Jane Austen" },
                    { 5, "Stephen Hawking" },
                    { 6, "Virginia Woolf" },
                    { 7, "Paulo Coelho" },
                    { 8, "Martin Fowler" },
                    { 9, "Isabel Allende" },
                    { 10, "Yuval Noah Harari" }
                });

            // Datos semilla para Estudiantes
            migrationBuilder.InsertData(
                table: "Estudiantes",
                columns: new[] { "EstudianteId", "Nombre", "Email", "FechaInscripcion" },
                values: new object[,]
                {
                    { 1, "Ana Martínez", "ana.martinez@email.com", DateTime.Now },
                    { 2, "Luis Pérez", "luis.perez@email.com", DateTime.Now },
                    { 3, "María Sánchez", "maria.sanchez@email.com", DateTime.Now },
                    { 4, "Carlos Rodríguez", "carlos.rodriguez@email.com", DateTime.Now },
                    { 5, "Laura González", "laura.gonzalez@email.com", DateTime.Now },
                    { 6, "Pedro Ramírez", "pedro.ramirez@email.com", DateTime.Now },
                    { 7, "Sofia Torres", "sofia.torres@email.com", DateTime.Now },
                    { 8, "Juan Méndez", "juan.mendez@email.com", DateTime.Now },
                    { 9, "Carmen Díaz", "carmen.diaz@email.com", DateTime.Now },
                    { 10, "Roberto Flores", "roberto.flores@email.com", DateTime.Now }
                });

            // Datos semilla para Libros
            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "LibroId", "Titulo", "ISBN", "AutorId", "CategoriaId" },
                values: new object[,]
                {
                    { 1, "Yo, Robot", "9788435034807", 1, 1 },
                    { 2, "Cien años de soledad", "9780307474728", 2, 4 },
                    { 3, "Clean Code", "9780132350884", 3, 3 },
                    { 4, "Orgullo y prejuicio", "9788497940246", 4, 4 },
                    { 5, "Breve historia del tiempo", "9788498387896", 5, 2 },
                    { 6, "Una habitación propia", "9788432233982", 6, 4 },
                    { 7, "El Alquimista", "9788408052951", 7, 4 },
                    { 8, "Patterns of Enterprise", "9780321127426", 8, 3 },
                    { 9, "La casa de los espíritus", "9788401352898", 9, 4 },
                    { 10, "Sapiens", "9788499926223", 10, 2 }
                });

            // Datos semilla para Préstamos
            migrationBuilder.InsertData(
                table: "Prestamos",
                columns: new[] { "PrestamoId", "LibroId", "EstudianteId", "FechaPrestamo", "FechaVencimiento" },
                values: new object[,]
                {
                    { 1, 1, 1, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-15) },
                    { 2, 2, 2, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-5) },
                    { 3, 3, 3, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(0) },
                    { 4, 4, 4, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(5) },
                    { 5, 5, 5, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(10) },
                    { 6, 6, 6, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(12) },
                    { 7, 7, 7, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(13) },
                    { 8, 8, 8, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(14) },
                    { 9, 9, 9, DateTime.Now, DateTime.Now.AddDays(15) },
                    { 10, 10, 10, DateTime.Now, DateTime.Now.AddDays(15) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Prestamos", keyColumn: "PrestamoId", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            migrationBuilder.DeleteData(table: "Libros", keyColumn: "LibroId", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            migrationBuilder.DeleteData(table: "Autores", keyColumn: "AutorId", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "CategoriaId", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            migrationBuilder.DeleteData(table: "Estudiantes", keyColumn: "EstudianteId", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }
    }
}
