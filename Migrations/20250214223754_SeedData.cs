using System;
using Microsoft.EntityFrameworkCore.Migrations;
using LibroManager.Models;

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
                columns: new[] { "CategoriaId", "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Ciencia Ficción", DateTime.Now },
                    { 2, "Historia", DateTime.Now },
                    { 3, "Programación", DateTime.Now },
                    { 4, "Literatura", DateTime.Now },
                    { 5, "Matemáticas", DateTime.Now },
                    { 6, "Psicología", DateTime.Now },
                    { 7, "Arte", DateTime.Now },
                    { 8, "Medicina", DateTime.Now },
                    { 9, "Economía", DateTime.Now },
                    { 10, "Idiomas", DateTime.Now }
                });

            // Datos semilla para Autores
            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "AutorId", "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Isaac Asimov", DateTime.Now },
                    { 2, "Gabriel García Márquez", DateTime.Now },
                    { 3, "Robert Martin", DateTime.Now },
                    { 4, "Jane Austen", DateTime.Now },
                    { 5, "Stephen Hawking", DateTime.Now },
                    { 6, "Virginia Woolf", DateTime.Now },
                    { 7, "Paulo Coelho", DateTime.Now },
                    { 8, "Martin Fowler", DateTime.Now },
                    { 9, "Isabel Allende", DateTime.Now },
                    { 10, "Yuval Noah Harari", DateTime.Now }
                });

            // Datos semilla para Estudiantes
            migrationBuilder.InsertData(
                table: "Estudiantes",
                columns: new[] { "EstudianteId", "Nombre", "Email", "FechaInscripcion", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Ana Martínez", "ana.martinez@email.com", DateTime.Now, DateTime.Now },
                    { 2, "Luis Pérez", "luis.perez@email.com", DateTime.Now, DateTime.Now },
                    { 3, "María Sánchez", "maria.sanchez@email.com", DateTime.Now, DateTime.Now },
                    { 4, "Carlos Rodríguez", "carlos.rodriguez@email.com", DateTime.Now, DateTime.Now },
                    { 5, "Laura González", "laura.gonzalez@email.com", DateTime.Now, DateTime.Now },
                    { 6, "Pedro Ramírez", "pedro.ramirez@email.com", DateTime.Now, DateTime.Now },
                    { 7, "Sofia Torres", "sofia.torres@email.com", DateTime.Now, DateTime.Now },
                    { 8, "Juan Méndez", "juan.mendez@email.com", DateTime.Now, DateTime.Now },
                    { 9, "Carmen Díaz", "carmen.diaz@email.com", DateTime.Now, DateTime.Now },
                    { 10, "Roberto Flores", "roberto.flores@email.com", DateTime.Now, DateTime.Now }
                });

            // Datos semilla para Libros
            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "LibroId", "Titulo", "ISBN", "AutorId", "CategoriaId", "Estado", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Yo, Robot", "9788435034807", 1, 1, EstadoLibro.Disponible, DateTime.Now },
                    { 2, "Cien años de soledad", "9780307474728", 2, 4, EstadoLibro.Disponible, DateTime.Now },
                    { 3, "Clean Code", "9780132350884", 3, 3, EstadoLibro.Prestado, DateTime.Now },
                    { 4, "Orgullo y prejuicio", "9788497940246", 4, 4, EstadoLibro.Prestado, DateTime.Now },
                    { 5, "Breve historia del tiempo", "9788498387896", 5, 2, EstadoLibro.Prestado, DateTime.Now },
                    { 6, "Una habitación propia", "9788432233982", 6, 4, EstadoLibro.Prestado, DateTime.Now },
                    { 7, "El Alquimista", "9788408052951", 7, 4, EstadoLibro.Prestado, DateTime.Now },
                    { 8, "Patterns of Enterprise", "9780321127426", 8, 3, EstadoLibro.Prestado, DateTime.Now },
                    { 9, "La casa de los espíritus", "9788401352898", 9, 4, EstadoLibro.Prestado, DateTime.Now },
                    { 10, "Sapiens", "9788499926223", 10, 2, EstadoLibro.Prestado, DateTime.Now }
                });

            // Datos semilla para Préstamos
            migrationBuilder.InsertData(
                table: "Prestamos",
                columns: new[] { "PrestamoId", "LibroId", "EstudianteId", "FechaPrestamo", "FechaVencimiento", "Estado", "FechaDevolucion", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, 1, 1, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-15), EstadoPrestamo.Concluido, DateTime.Now.AddDays(-15), DateTime.Now },
                    { 2, 2, 2, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-5), EstadoPrestamo.Concluido, DateTime.Now.AddDays(-5), DateTime.Now },
                    { 3, 3, 3, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(0), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 4, 4, 4, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(5), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 5, 5, 5, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(10), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 6, 6, 6, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(12), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 7, 7, 7, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(13), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 8, 8, 8, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(14), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 9, 9, 9, DateTime.Now, DateTime.Now.AddDays(15), EstadoPrestamo.Activo, null, DateTime.Now },
                    { 10, 10, 10, DateTime.Now, DateTime.Now.AddDays(15), EstadoPrestamo.Activo, null, DateTime.Now }
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
