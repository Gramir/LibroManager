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
                columns: new[] { "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { "Gabriel García Márquez", DateTime.Now },
                    { "Jorge Luis Borges", DateTime.Now },
                    { "Mario Vargas Llosa", DateTime.Now },
                    { "Isabel Allende", DateTime.Now },
                    { "Julio Cortázar", DateTime.Now }
                }
            );

            // Insertar Categorías
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Nombre", "FechaCreacion" },
                values: new object[,]
                {
                    { "Novela", DateTime.Now },
                    { "Cuento", DateTime.Now },
                    { "Poesía", DateTime.Now },
                    { "Ensayo", DateTime.Now },
                    { "Ciencia Ficción", DateTime.Now }
                }
            );

            // Insertar Estudiantes
            migrationBuilder.InsertData(
                table: "Estudiantes",
                columns: new[] { "Nombre", "Email", "FechaInscripcion", "FechaCreacion" },
                values: new object[,]
                {
                    { "Juan Pérez", "juan.perez@email.com", DateTime.Now, DateTime.Now },
                    { "María García", "maria.garcia@email.com", DateTime.Now, DateTime.Now },
                    { "Carlos López", "carlos.lopez@email.com", DateTime.Now, DateTime.Now },
                    { "Ana Martínez", "ana.martinez@email.com", DateTime.Now, DateTime.Now },
                    { "Pedro Rodríguez", "pedro.rodriguez@email.com", DateTime.Now, DateTime.Now }
                }
            );

            // Insertar Libros
            migrationBuilder.Sql(@"
                INSERT INTO Libros (Titulo, ISBN, AutorId, CategoriaId, Estado, FechaCreacion)
                SELECT 'Cien años de soledad', '9780307474728', AutorId, (SELECT CategoriaId FROM Categorias WHERE Nombre = 'Novela'), 0, GETDATE()
                FROM Autores WHERE Nombre = 'Gabriel García Márquez';

                INSERT INTO Libros (Titulo, ISBN, AutorId, CategoriaId, Estado, FechaCreacion)
                SELECT 'El Aleph', '9780142437883', AutorId, (SELECT CategoriaId FROM Categorias WHERE Nombre = 'Cuento'), 0, GETDATE()
                FROM Autores WHERE Nombre = 'Jorge Luis Borges';

                INSERT INTO Libros (Titulo, ISBN, AutorId, CategoriaId, Estado, FechaCreacion)
                SELECT 'La ciudad y los perros', '9788466333215', AutorId, (SELECT CategoriaId FROM Categorias WHERE Nombre = 'Novela'), 0, GETDATE()
                FROM Autores WHERE Nombre = 'Mario Vargas Llosa';

                INSERT INTO Libros (Titulo, ISBN, AutorId, CategoriaId, Estado, FechaCreacion)
                SELECT 'La casa de los espíritus', '9780525433477', AutorId, (SELECT CategoriaId FROM Categorias WHERE Nombre = 'Novela'), 0, GETDATE()
                FROM Autores WHERE Nombre = 'Isabel Allende';

                INSERT INTO Libros (Titulo, ISBN, AutorId, CategoriaId, Estado, FechaCreacion)
                SELECT 'Rayuela', '9788437604572', AutorId, (SELECT CategoriaId FROM Categorias WHERE Nombre = 'Novela'), 0, GETDATE()
                FROM Autores WHERE Nombre = 'Julio Cortázar';
            ");

            // Insertar algunos Préstamos activos
            migrationBuilder.Sql(@"
                INSERT INTO Prestamos (LibroId, EstudianteId, FechaPrestamo, FechaVencimiento, Estado, FechaCreacion)
                SELECT TOP 2 
                    l.LibroId,
                    (SELECT TOP 1 EstudianteId FROM Estudiantes ORDER BY NEWID()),
                    GETDATE(),
                    DATEADD(day, 7, GETDATE()),
                    0, -- Estado Activo
                    GETDATE()
                FROM Libros l
                ORDER BY NEWID();

                -- Actualizar el estado de los libros prestados
                UPDATE Libros
                SET Estado = 1 -- EstadoLibro.Prestado
                WHERE LibroId IN (SELECT LibroId FROM Prestamos);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Prestamos");
            migrationBuilder.Sql("DELETE FROM Libros");
            migrationBuilder.Sql("DELETE FROM Estudiantes");
            migrationBuilder.Sql("DELETE FROM Categorias");
            migrationBuilder.Sql("DELETE FROM Autores");
        }
    }
}
