namespace NorthWind.Membership.Entities.ValueObjects
{
    public class Endpoints
    {
        // User Registration & Login
        public const string Register = $"/user/{nameof(Register)}";
        public const string Login = $"/user/{nameof(Login)}";

        // User Management - Query
        public const string GetAllUsers = $"/user/{nameof(GetAllUsers)}";
        public const string GetLockedOutUsers = $"/user/{nameof(GetLockedOutUsers)}";

        // User Management - Commands
        public const string UnlockUser = $"/user/{nameof(UnlockUser)}";
        public const string ChangeUserRole = $"/user/{nameof(ChangeUserRole)}";
        public const string UpdateUser = $"/user/{nameof(UpdateUser)}";
        public const string DeleteUser = $"/user/{nameof(DeleteUser)}";

        public const string GetUserById = "/api/users/{userId}";
        public const string Logout = "/api/auth/logout";

    }
}
