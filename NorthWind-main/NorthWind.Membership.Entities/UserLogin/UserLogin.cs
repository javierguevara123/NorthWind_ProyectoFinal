namespace NorthWind.Membership.Entities.UserLogin
{
    public class UserCredentialsDto(string email, string password)
    {
        public string Email => email;
        public string Password => password;
    }
}
