namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class GetUserByIdResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDetailDto User { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class UserDetailDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }  // ← AGREGAR
        public string Role { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }  // ← AGREGAR
    }
}
