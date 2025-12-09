namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class DeleteUserDto
    {
        public string Email { get; set; } // Email del usuario a eliminar
        public string CurrentUserEmail { get; set; } // Email del usuario que ejecuta la acción
    }
}
