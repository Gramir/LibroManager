namespace LibroManager.Constants;

public static class RoleConstants
{
    public const string AdminRole = "Admin";
    public const string LibrarianRole = "Librarian";

    public static class DefaultPermissions
    {
        public static readonly string[] AdminPermissions = {
            Permissions.Libros.Create, Permissions.Libros.Read, Permissions.Libros.Update, Permissions.Libros.Delete, Permissions.Libros.Manage,
            Permissions.Autores.Create, Permissions.Autores.Read, Permissions.Autores.Update, Permissions.Autores.Delete,
            Permissions.Prestamos.Create, Permissions.Prestamos.Read, Permissions.Prestamos.Update, Permissions.Prestamos.Delete, Permissions.Prestamos.Manage,
            Permissions.Estudiantes.Create, Permissions.Estudiantes.Read, Permissions.Estudiantes.Update, Permissions.Estudiantes.Delete,
            Permissions.Ubicaciones.Create, Permissions.Ubicaciones.Read, Permissions.Ubicaciones.Update, Permissions.Ubicaciones.Delete,
            Permissions.Categorias.Create, Permissions.Categorias.Read, Permissions.Categorias.Update, Permissions.Categorias.Delete,
            Permissions.Users.ManageUsers
        };

        public static readonly string[] LibrarianPermissions = {
            Permissions.Libros.Read, Permissions.Libros.Update,
            Permissions.Autores.Read,
            Permissions.Prestamos.Create, Permissions.Prestamos.Read, Permissions.Prestamos.Update,
            Permissions.Estudiantes.Read,
            Permissions.Ubicaciones.Read,
            Permissions.Categorias.Read
        };
    }

    public static class Permissions
    {
        public static class Libros
        {
            public const string Create = "Libros.Create";
            public const string Read = "Libros.Read";
            public const string Update = "Libros.Update";
            public const string Delete = "Libros.Delete";
            public const string Manage = "Libros.Manage";
        }

        public static class Autores
        {
            public const string Create = "Autores.Create";
            public const string Read = "Autores.Read";
            public const string Update = "Autores.Update";
            public const string Delete = "Autores.Delete";
        }

        public static class Prestamos
        {
            public const string Create = "Prestamos.Create";
            public const string Read = "Prestamos.Read";
            public const string Update = "Prestamos.Update";
            public const string Delete = "Prestamos.Delete";
            public const string Manage = "Prestamos.Manage";
        }

        public static class Estudiantes
        {
            public const string Create = "Estudiantes.Create";
            public const string Read = "Estudiantes.Read";
            public const string Update = "Estudiantes.Update";
            public const string Delete = "Estudiantes.Delete";
        }

        public static class Ubicaciones
        {
            public const string Create = "Ubicaciones.Create";
            public const string Read = "Ubicaciones.Read";
            public const string Update = "Ubicaciones.Update";
            public const string Delete = "Ubicaciones.Delete";
        }

        public static class Categorias
        {
            public const string Create = "Categorias.Create";
            public const string Read = "Categorias.Read";
            public const string Update = "Categorias.Update";
            public const string Delete = "Categorias.Delete";
        }

        public static class Users
        {
            public const string ManageRoles = "Users.ManageRoles";
            public const string ManageUsers = "Users.ManageUsers";
        }
    }
}