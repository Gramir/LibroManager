
![CI](https://github.com/Gramir/LibroManager/actions/workflows/ci.yml/badge.svg)

# LibroManager

Library management system developed with .NET 8 and Blazor Server.

## Description

LibroManager is a modern web application for library management, allowing you to manage books, authors, categories, students, and loans. Built with the latest Microsoft technologies, it offers an intuitive interface and complete features for library operations.

## Main Features

- Complete book management with copy control
- Author registration and tracking
- Book categorization
- Student management
- Loan system with due dates
- Automatic availability validations
- Responsive and modern interface

## Technologies Used

- .NET 8
- Blazor Server
- Entity Framework Core 9.0.2
- SQL Server
- xUnit for unit testing

## Project Structure

- `DTOs/`: Data Transfer Objects
- `Models/`: Domain models
- `Repositories/`: Repository pattern implementation
- `Services/`: Business logic
- `Tests/`: Unit and automated tests with Playwright
  - `Playwright/`: End-to-end automated tests

## Main Models

### Book
- Title
- ISBN (unique)
- Author
- Category
- Number of copies
- Status (Available/Loaned/Lost)

### Loan
- Book
- Student
- Loan date
- Due date
- Status (Active/Completed/Expired)

### Student
- Name
- Email (unique)
- Enrollment date
- Loan history

## Database Features

- Foreign keys with restricted deletion
- Unique indexes for ISBN and email
- Database-level validations
- Seed data included

## Setup

1. Make sure you have installed:
  - SQL Server
  - Visual Studio 2022 (recommended)

2. Configure the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
   "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LibroManagerDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Run migrations:
```bash
dotnet ef database update
```


## Unit Testing

Unit tests are located in the `LibroManager.Tests.Unit` project. These tests cover business logic, models, and repository functionality.

### Running unit tests
1. Restore dependencies:
  ```sh
  dotnet restore
  ```
2. Run unit tests:
  ```sh
  dotnet test LibroManager.Tests.Unit
  ```

You can also run all tests (unit and E2E) with:
```sh
dotnet test
```

## Automated Testing with Playwright

The project includes end-to-end automated tests using [Microsoft Playwright](https://playwright.dev/dotnet/).

### What is tested?
- Main application flows
- Navigation, authentication, and visual validations

### Running Playwright tests
1. Install Playwright dependencies:
  ```bash
  dotnet tool install --global Microsoft.Playwright.CLI
  playwright install
  ```
2. Run the tests:
  ```bash
  dotnet test Tests/Playwright
  ```

Check the `Tests/Playwright` folder for test files and automated pages.

## Development Features

- Dependency injection
- Repository and Unit of Work patterns
- AutoMapper for object mapping
- Custom validations
- Extensive unit tests
- End-to-end tests with Playwright
- Hot Reload enabled

## CI/CD Integration with GitHub Actions

This project includes automatic continuous integration (CI):
- Every push or pull request to the `main` or `Testing` branches runs build and tests.
- The workflow is in `.github/workflows/ci.yml`.
- No additional configuration required: just upload the file and push to the configured branches.
- You can see the status and logs in the "Actions" tab of your GitHub repository.

## Quick Installation and Run Guide

1. Clone the repository:
  ```sh
  git clone https://github.com/Gramir/LibroManager.git
  ```
2. Install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
3. Restore dependencies:
  ```sh
  dotnet restore
  ```
4. Build the project:
  ```sh
  dotnet build
  ```
5. Run the application:
  ```sh
  dotnet run
  ```

## How to Contribute

1. Fork the repository
2. Create a branch for your change:
  ```sh
  git checkout -b feature/my-change
  ```
3. Make your changes and tests
4. Commit and push:
  ```sh
  git commit -m "Add new feature"
  git push origin feature/my-change
  ```
5. Open a Pull Request on GitHub

## Contact and Support

For questions, suggestions, or bug reports, open an issue on GitHub or contact the repository owner.

---

MIT License

Author: Gilberto Ramirez

---

# LibroManager (Español)

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
- xUnit para pruebas unitarias

## Estructura del Proyecto

- `DTOs/`: Objetos de transferencia de datos
- `Models/`: Modelos de dominio
- `Repositories/`: Implementación del patrón repositorio
- `Services/`: Lógica de negocio
- `Tests/`: Pruebas unitarias y pruebas automatizadas con Playwright
  - `Playwright/`: Pruebas end-to-end automatizadas

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


## Pruebas Unitarias

Las pruebas unitarias se encuentran en el proyecto `LibroManager.Tests.Unit`. Estas pruebas cubren la lógica de negocio, modelos y funcionalidad de los repositorios.

### Ejecución de pruebas unitarias
1. Restaura las dependencias:
  ```sh
  dotnet restore
  ```
2. Ejecuta las pruebas unitarias:
  ```sh
  dotnet test LibroManager.Tests.Unit
  ```

También puedes ejecutar todas las pruebas (unitarias y E2E) con:
```sh
dotnet test
```

## Pruebas Automatizadas con Playwright

El proyecto incluye pruebas end-to-end automatizadas utilizando [Microsoft Playwright](https://playwright.dev/dotnet/).

### ¿Qué se prueba?
- Flujos principales de la aplicación web
- Navegación, autenticación y validaciones visuales

### Ejecución de pruebas Playwright
1. Instala las dependencias de Playwright:
  ```bash
  dotnet tool install --global Microsoft.Playwright.CLI
  playwright install
  ```
2. Ejecuta las pruebas:
  ```bash
  dotnet test Tests/Playwright
  ```

Consulta la carpeta `Tests/Playwright` para ver los archivos de las pruebas y las páginas automatizadas.

## Características de Desarrollo

- Inyección de dependencias
- Patrón Repository y Unit of Work
- AutoMapper para mapeo de objetos
- Validaciones personalizadas
- Pruebas unitarias extensivas
- Pruebas end-to-end con Playwright
- Hot Reload habilitado

## Integración CI/CD con GitHub Actions

Este proyecto incluye integración continua (CI) automática:
- Cada push o pull request a las ramas `main` o `Testing` ejecuta compilación y pruebas.
- El workflow está en `.github/workflows/ci.yml`.
- No requiere configuración adicional para funcionar: solo sube el archivo y haz push a las ramas configuradas.
- Puedes ver el estado y los logs en la pestaña "Actions" de tu repositorio en GitHub.

## Guía rápida de instalación y ejecución

1. Clona el repositorio:
  ```sh
  git clone https://github.com/Gramir/LibroManager.git
  ```
2. Instala .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
3. Restaura dependencias:
  ```sh
  dotnet restore
  ```
4. Compila el proyecto:
  ```sh
  dotnet build
  ```
5. Ejecuta la aplicación:
  ```sh
  dotnet run
  ```

## Cómo contribuir

1. Haz un fork del repositorio
2. Crea una rama para tu cambio:
  ```sh
  git checkout -b feature/mi-cambio
  ```
3. Realiza tus cambios y pruebas
4. Haz commit y push:
  ```sh
  git commit -m "Agrega nueva funcionalidad"
  git push origin feature/mi-cambio
  ```
5. Abre un Pull Request en GitHub

## Contacto y soporte

Para dudas, sugerencias o reportes de bugs, abre un issue en GitHub o contacta al propietario del repo.

---

MIT License

Autor: Gilberto Ramirez
