namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class GetUserByIdDto
    {
        public string UserId { get; set; }
        public string CurrentUserEmail { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
