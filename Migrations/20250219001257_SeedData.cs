using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insertar Autores
            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "AutorId", "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Gabriel García Márquez", DateTime.Now },
                    { 2, "Jorge Luis Borges", DateTime.Now },
                    { 3, "Isabel Allende", DateTime.Now },
                    { 4, "Mario Vargas Llosa", DateTime.Now },
                    { 5, "Pablo Neruda", DateTime.Now },
                    { 6, "Julio Cortázar", DateTime.Now },
                    { 7, "Octavio Paz", DateTime.Now },
                    { 8, "Carlos Fuentes", DateTime.Now },
                    { 9, "Miguel Ángel Asturias", DateTime.Now },
                    { 10, "Juan Rulfo", DateTime.Now }
                });

            // Insertar Categorías
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "CategoriaId", "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Ficción", DateTime.Now },
                    { 2, "No Ficción", DateTime.Now },
                    { 3, "Literatura Clásica", DateTime.Now },
                    { 4, "Poesía", DateTime.Now },
                    { 5, "Drama", DateTime.Now },
                    { 6, "Ciencia Ficción", DateTime.Now },
                    { 7, "Misterio", DateTime.Now },
                    { 8, "Biografía", DateTime.Now },
                    { 9, "Historia", DateTime.Now },
                    { 10, "Ensayo", DateTime.Now }
                });

            // Insertar Estudiantes
            migrationBuilder.InsertData(
                table: "Estudiantes",
                columns: new[] { "EstudianteId", "Nombre", "Email", "FechaInscripcion", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Juan Pérez", "juan.perez@email.com", DateTime.Now, DateTime.Now },
                    { 2, "María García", "maria.garcia@email.com", DateTime.Now, DateTime.Now },
                    { 3, "Carlos López", "carlos.lopez@email.com", DateTime.Now, DateTime.Now },
                    { 4, "Ana Martínez", "ana.martinez@email.com", DateTime.Now, DateTime.Now },
                    { 5, "Luis Rodríguez", "luis.rodriguez@email.com", DateTime.Now, DateTime.Now },
                    { 6, "Carmen Silva", "carmen.silva@email.com", DateTime.Now, DateTime.Now },
                    { 7, "Pedro Ramírez", "pedro.ramirez@email.com", DateTime.Now, DateTime.Now },
                    { 8, "Sofia Torres", "sofia.torres@email.com", DateTime.Now, DateTime.Now },
                    { 9, "Miguel Sánchez", "miguel.sanchez@email.com", DateTime.Now, DateTime.Now },
                    { 10, "Laura Díaz", "laura.diaz@email.com", DateTime.Now, DateTime.Now }
                });

            // Insertar Libros
            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "LibroId", "Titulo", "ISBN", "AutorId", "CategoriaId", "Estado", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, "Cien años de soledad", "9780307474728", 1, 3, 0, DateTime.Now },
                    { 2, "El Aleph", "9788499089515", 2, 1, 0, DateTime.Now },
                    { 3, "La casa de los espíritus", "9780525433477", 3, 1, 0, DateTime.Now },
                    { 4, "La ciudad y los perros", "9788420471839", 4, 3, 0, DateTime.Now },
                    { 5, "Veinte poemas de amor", "9780307474735", 5, 4, 0, DateTime.Now },
                    { 6, "Rayuela", "9788420471846", 6, 1, 0, DateTime.Now },
                    { 7, "El laberinto de la soledad", "9780307474742", 7, 10, 0, DateTime.Now },
                    { 8, "La muerte de Artemio Cruz", "9788420471853", 8, 1, 0, DateTime.Now },
                    { 9, "El señor presidente", "9780307474759", 9, 3, 0, DateTime.Now },
                    { 10, "Pedro Páramo", "9788420471860", 10, 3, 0, DateTime.Now }
                });

            // Insertar Préstamos
            migrationBuilder.InsertData(
                table: "Prestamos",
                columns: new[] { "PrestamoId", "LibroId", "EstudianteId", "FechaPrestamo", "FechaVencimiento", "Estado", "FechaCreacion" },
                values: new object[,]
                {
                    { 1, 1, 1, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(2), 0, DateTime.Now },
                    { 2, 3, 2, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(4), 0, DateTime.Now },
                    { 3, 5, 3, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(-8), 1, DateTime.Now },
                    { 4, 7, 4, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-13), 2, DateTime.Now }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Prestamos",
                keyColumn: "PrestamoId",
                keyValues: new object[] { 1, 2, 3, 4 });

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "LibroId",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "AutorId",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "CategoriaId",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            migrationBuilder.DeleteData(
                table: "Estudiantes",
                keyColumn: "EstudianteId",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }
    }
}
