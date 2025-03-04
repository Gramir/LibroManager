using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibroManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSerialsToIsbnFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizar los seriales existentes al nuevo formato ISBN-Número
            // LibroId 1-3 (Cien años de soledad)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780307474728-1' WHERE LibroId = 1");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780307474728-2' WHERE LibroId = 9");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780307474728-3' WHERE LibroId = 10");

            // LibroId 2,11-13 (Harry Potter y la piedra filosofal)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884452-1' WHERE LibroId = 2");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884452-2' WHERE LibroId = 11");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884452-3' WHERE LibroId = 12");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884452-4' WHERE LibroId = 13");

            // LibroId 3,14 (El resplandor)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788497593793-1' WHERE LibroId = 3");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788497593793-2' WHERE LibroId = 14");

            // LibroId 4,15,16 (La casa de los espíritus)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780525433477-1' WHERE LibroId = 4");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780525433477-2' WHERE LibroId = 15");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780525433477-3' WHERE LibroId = 16");

            // LibroId 5 (Tokio blues)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788483835043-1' WHERE LibroId = 5");

            // LibroId 6 (El amor en los tiempos del cólera)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9780307387264-1' WHERE LibroId = 6");

            // LibroId 7,17,18 (Harry Potter y la cámara secreta)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884957-1' WHERE LibroId = 7");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884957-2' WHERE LibroId = 17");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9788478884957-3' WHERE LibroId = 18");

            // LibroId 8,19 (It)
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9781501142970-1' WHERE LibroId = 8");
            migrationBuilder.Sql("UPDATE Libros SET Serial = '9781501142970-2' WHERE LibroId = 19");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restaurar los seriales originales
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'GAB-SOL-001' WHERE LibroId = 1");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'GAB-SOL-002' WHERE LibroId = 9");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'GAB-SOL-003' WHERE LibroId = 10");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP1-001' WHERE LibroId = 2");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP1-002' WHERE LibroId = 11");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP1-003' WHERE LibroId = 12");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP1-004' WHERE LibroId = 13");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'KIN-RES-001' WHERE LibroId = 3");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'KIN-RES-002' WHERE LibroId = 14");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ALL-ESP-001' WHERE LibroId = 4");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ALL-ESP-002' WHERE LibroId = 15");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ALL-ESP-003' WHERE LibroId = 16");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'MUR-TOK-001' WHERE LibroId = 5");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'GAB-AMO-001' WHERE LibroId = 6");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP2-001' WHERE LibroId = 7");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP2-002' WHERE LibroId = 17");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'ROW-HP2-003' WHERE LibroId = 18");

            migrationBuilder.Sql("UPDATE Libros SET Serial = 'KIN-IT-001' WHERE LibroId = 8");
            migrationBuilder.Sql("UPDATE Libros SET Serial = 'KIN-IT-002' WHERE LibroId = 19");
        }
    }
}