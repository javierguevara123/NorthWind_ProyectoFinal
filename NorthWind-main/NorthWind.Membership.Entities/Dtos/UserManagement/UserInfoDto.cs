namespace NorthWind.Membership.Entities.Dtos.UserManagement
{
    public class UserInfoDto
    {
        public string Id { get; set; }  // ← AGREGAR ESTA PROPIEDAD SI NO LA TIENE
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
