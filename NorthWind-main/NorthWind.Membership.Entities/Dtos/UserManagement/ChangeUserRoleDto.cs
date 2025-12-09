namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class ChangeUserRoleDto
    {
        public string Email { get; set; }
        public string NewRole { get; set; } // "SuperUser", "Administrator", "Employee"
    }
}
