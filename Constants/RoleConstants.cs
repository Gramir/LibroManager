namespace LibroManager.Constants;

public static class RoleConstants
{
    public const string AdminRole = "Admin";
    public const string BibliotecarioRole = "Bibliotecario";

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

        public static class Users
        {
            public const string ManageRoles = "Users.ManageRoles";
            public const string ManageUsers = "Users.ManageUsers";
        }
    }
}