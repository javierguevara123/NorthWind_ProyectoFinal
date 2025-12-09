namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class UpdateUserDto
    {
        public string Email { get; set; } // Email del usuario a actualizar
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }
        public string NewPassword { get; set; } // Opcional: null si no se cambia
        public string CurrentUserEmail { get; set; } // Email del usuario que ejecuta la acción
    }
}
