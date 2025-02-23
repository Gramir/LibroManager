# LibroManager

Sistema de gestión de biblioteca desarrollado con .NET 8 y Blazor Server.

## Descripción

LibroManager es una aplicación web moderna para la gestión de bibliotecas que permite administrar libros, autores, categorías, estudiantes y préstamos. Desarrollada con las últimas tecnologías de Microsoft, ofrece una interfaz intuitiva y funcionalidades completas para el manejo de una biblioteca.

## Características Principales

- Gestión completa de libros con control de ejemplares
- Registro y seguimiento de autores
- Categorización de libros
- Gestión de estudiantes
- Sistema de préstamos con fechas de vencimiento
- Validaciones automáticas de disponibilidad
- Interfaz responsiva y moderna

## Tecnologías Utilizadas

- .NET 8
- Blazor Server
- Entity Framework Core 9.0.2
- SQL Server
- AutoMapper
- xUnit para pruebas unitarias

## Estructura del Proyecto

- `Components/`: Componentes Blazor y páginas de la aplicación
- `Data/`: Contexto de base de datos y configuraciones
- `DTOs/`: Objetos de transferencia de datos
- `Models/`: Modelos de dominio
- `Repositories/`: Implementación del patrón repositorio
- `Services/`: Lógica de negocio
- `Tests/`: Pruebas unitarias

## Modelos Principales

### Libro
- Título
- ISBN (único)
- Autor
- Categoría
- Número de ejemplares
- Estado (Disponible/Prestado/Perdido)

### Préstamo
- Libro
- Estudiante
- Fecha de préstamo
- Fecha de vencimiento
- Estado (Activo/Concluido/Expirado)

### Estudiante
- Nombre
- Email (único)
- Fecha de inscripción
- Historial de préstamos

## Características de la Base de Datos

- Claves foráneas con eliminación restringida
- Índices únicos para ISBN y email
- Validaciones a nivel de base de datos
- Datos semilla incluidos

## Configuración

1. Asegúrate de tener instalado:
   - .NET 8 SDK
   - SQL Server
   - Visual Studio 2022 (recomendado)

2. Configura la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LibroManagerDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Ejecuta las migraciones:
```bash
dotnet ef database update
```

## Características de Desarrollo

- Inyección de dependencias
- Patrón Repository y Unit of Work
- AutoMapper para mapeo de objetos
- Validaciones personalizadas
- Pruebas unitarias extensivas
- Hot Reload habilitado

## Licencia

MIT License

## Autor

Gilberto Ramirez