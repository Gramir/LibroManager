
![CI](https://github.com/Gramir/LibroManager/actions/workflows/ci.yml/badge.svg)

# LibroManager

Library management system developed with .NET 8 and Blazor Server.

## Description

LibroManager is a modern web application for library management, allowing you to manage books, authors, categories, students, and loans. Built with the latest Microsoft technologies, it offers an intuitive interface and complete features for library operations.

**Unique Feature**: This project demonstrates a dual E2E testing architecture, implementing both traditional Page Object Model (POM) and modern Screenplay Pattern approaches side-by-side for direct comparison and learning purposes.

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
- **Playwright** for E2E testing (both POM and Screenplay patterns)

## Project Structure

- `DTOs/`: Data Transfer Objects
- `Models/`: Domain models
- `Repositories/`: Repository pattern implementation
- `Services/`: Business logic
- `Tests/`: Unit and automated tests with Playwright
  - `LibroManager.Tests.Unit/`: Unit tests
  - `LibroManager.Tests.E2E/`: End-to-end tests
    - `POM/`: Page Object Model implementation
    - `Screenplay/`: Screenplay Pattern implementation
    - `Helpers/`: Shared test utilities

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

## Screenplay Pattern Implementation

This project features a unique dual-architecture approach for E2E testing, implementing both the traditional **Page Object Model (POM)** and the modern **Screenplay Pattern** side-by-side. This allows for direct comparison between both architectural approaches.

### What is the Screenplay Pattern?

The Screenplay Pattern is a modern approach to automated testing that focuses on **user behavior** rather than technical implementation details. It treats tests as **stories** where:

- **Actors** (users) perform **Tasks** (actions)
- **Actors** ask **Questions** (assertions) about the application state
- **Abilities** provide the technical capabilities (like browsing the web)
- **UI Models** define the interface elements

### Benefits of Screenplay Pattern

- **Business-focused**: Tests read like user stories
- **Reusable**: Tasks and Questions can be composed and reused
- **Maintainable**: Changes to UI don't break multiple tests
- **Extensible**: Easy to add new capabilities and behaviors
- **Type-safe**: Strong typing prevents runtime errors

### Screenplay Architecture

```
Screenplay/
├── Core/                          # Core framework
│   ├── Abilities/                 # Actor capabilities (BrowseTheWeb)
│   ├── Actors/                    # Actor implementation
│   ├── Exceptions/                # Custom exceptions
│   ├── Interactions/              # Tasks and Questions
│   │   ├── Tasks/                 # Atomic actions
│   │   └── Questions/             # Assertions
│   └── Logging/                   # Action logging
├── Tasks/                         # High-level business tasks
├── UI/                            # UI element definitions
└── Tests/                         # Screenplay test implementations
```

### Running Screenplay Tests

All Screenplay tests are marked with "(Screenplay)" in their display names:

```bash
# Run all tests (POM + Screenplay)
dotnet test LibroManager.Tests.E2E

# Run only Screenplay tests
dotnet test LibroManager.Tests.E2E --filter "DisplayName~Screenplay"

# Run only POM tests
dotnet test LibroManager.Tests.E2E --filter "DisplayName!~Screenplay"
```

### Screenplay Test Example

```csharp
[Fact(DisplayName = "Admin can login successfully (Screenplay)")]
public async Task Admin_Can_Login_Successfully()
{
    await RunScreenplayTestWithPageAsync(
        actorName: "Admin",
        testName: nameof(Admin_Can_Login_Successfully),
        screenplay: async (actor, page) =>
        {
            // WHEN: Admin logs in
            await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
            
            // THEN: Should be logged in
            await actor.AsksForAsync(TheVisibility.Of(MainPageUI.UserLogged(page)));
        });
}
```

### Comparison: POM vs Screenplay

| Aspect | Page Object Model | Screenplay Pattern |
|--------|------------------|-------------------|
| Focus | Technical implementation | Business behavior |
| Test readability | `page.Login("admin", "pass")` | `actor.AttemptsTo(LoginAsUser.AsAdmin())` |
| Reusability | Page methods | Composable Tasks/Questions |
| Maintenance | UI changes break tests | UI changes isolated |
| Learning curve | Low | Medium |

Both approaches coexist in this project, allowing teams to:
- Compare implementation approaches
- Migrate gradually from POM to Screenplay
- Choose the best pattern for different scenarios

## Development Features

- Dependency injection
- Repository and Unit of Work patterns
- AutoMapper for object mapping
- Custom validations
- Extensive unit tests
- **Dual E2E testing architecture**: Page Object Model (POM) and Screenplay Pattern
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

**Característica Única**: Este proyecto demuestra una arquitectura dual de pruebas E2E, implementando tanto el tradicional Page Object Model (POM) como el moderno Patrón Screenplay lado a lado para comparación directa y fines de aprendizaje.

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
- **Playwright** para pruebas E2E (patrones POM y Screenplay)

## Estructura del Proyecto

- `DTOs/`: Objetos de transferencia de datos
- `Models/`: Modelos de dominio
- `Repositories/`: Implementación del patrón repositorio
- `Services/`: Lógica de negocio
- `Tests/`: Pruebas unitarias y pruebas automatizadas con Playwright
  - `LibroManager.Tests.Unit/`: Pruebas unitarias
  - `LibroManager.Tests.E2E/`: Pruebas end-to-end
    - `POM/`: Implementación Page Object Model
    - `Screenplay/`: Implementación Patrón Screenplay
    - `Helpers/`: Utilidades compartidas de pruebas

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

## Implementación del Patrón Screenplay

Este proyecto cuenta con un enfoque único de arquitectura dual para pruebas E2E, implementando tanto el tradicional **Page Object Model (POM)** como el moderno **Patrón Screenplay** lado a lado. Esto permite comparar directamente ambos enfoques arquitectónicos.

### ¿Qué es el Patrón Screenplay?

El Patrón Screenplay es un enfoque moderno para las pruebas automatizadas que se centra en el **comportamiento del usuario** en lugar de los detalles técnicos de implementación. Trata las pruebas como **historias** donde:

- **Actores** (usuarios) realizan **Tareas** (acciones)
- **Actores** preguntan **Preguntas** (afirmaciones) sobre el estado de la aplicación
- **Habilidades** proporcionan las capacidades técnicas (como navegar por la web)
- **Modelos de UI** definen los elementos de interfaz

### Beneficios del Patrón Screenplay

- **Enfocado en negocio**: Las pruebas se leen como historias de usuario
- **Reutilizable**: Las Tareas y Preguntas se pueden componer y reutilizar
- **Mantenible**: Los cambios en la UI no rompen múltiples pruebas
- **Extensible**: Fácil agregar nuevas capacidades y comportamientos
- **Type-safe**: Tipado fuerte previene errores en tiempo de ejecución

### Arquitectura Screenplay

```
Screenplay/
├── Core/                          # Framework central
│   ├── Abilities/                 # Capacidades de actores (BrowseTheWeb)
│   ├── Actors/                    # Implementación de actores
│   ├── Exceptions/                # Excepciones personalizadas
│   ├── Interactions/              # Tareas y Preguntas
│   │   ├── Tasks/                 # Acciones atómicas
│   │   └── Questions/             # Afirmaciones
│   └── Logging/                   # Registro de acciones
├── Tasks/                         # Tareas de alto nivel de negocio
├── UI/                            # Definiciones de elementos UI
└── Tests/                         # Implementaciones de pruebas Screenplay
```

### Ejecución de Pruebas Screenplay

Todas las pruebas Screenplay están marcadas con "(Screenplay)" en sus nombres:

```bash
# Ejecutar todas las pruebas (POM + Screenplay)
dotnet test LibroManager.Tests.E2E

# Ejecutar solo pruebas Screenplay
dotnet test LibroManager.Tests.E2E --filter "DisplayName~Screenplay"

# Ejecutar solo pruebas POM
dotnet test LibroManager.Tests.E2E --filter "DisplayName!~Screenplay"
```

### Ejemplo de Prueba Screenplay

```csharp
[Fact(DisplayName = "El admin puede iniciar sesión correctamente (Screenplay)")]
public async Task Admin_Can_Login_Successfully()
{
    await RunScreenplayTestWithPageAsync(
        actorName: "Admin",
        testName: nameof(Admin_Can_Login_Successfully),
        screenplay: async (actor, page) =>
        {
            // CUANDO: El admin inicia sesión
            await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
            
            // ENTONCES: Debería estar logueado
            await actor.AsksForAsync(TheVisibility.Of(MainPageUI.UserLogged(page)));
        });
}
```

### Comparación: POM vs Screenplay

| Aspecto | Page Object Model | Patrón Screenplay |
|---------|------------------|-------------------|
| Enfoque | Implementación técnica | Comportamiento de negocio |
| Legibilidad | `page.Login("admin", "pass")` | `actor.AttemptsTo(LoginAsUser.AsAdmin())` |
| Reutilización | Métodos de página | Tareas/Preguntas componibles |
| Mantenimiento | Cambios UI rompen tests | Cambios UI aislados |
| Curva de aprendizaje | Baja | Media |

Ambos enfoques coexisten en este proyecto, permitiendo a los equipos:
- Comparar enfoques de implementación
- Migrar gradualmente de POM a Screenplay
- Elegir el mejor patrón para diferentes escenarios

## Características de Desarrollo

- Inyección de dependencias
- Patrón Repository y Unit of Work
- AutoMapper para mapeo de objetos
- Validaciones personalizadas
- Pruebas unitarias extensivas
- **Arquitectura dual de pruebas E2E**: Page Object Model (POM) y Patrón Screenplay
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
