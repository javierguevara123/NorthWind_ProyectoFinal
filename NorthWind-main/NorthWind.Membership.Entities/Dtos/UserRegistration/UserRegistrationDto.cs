namespace NorthWind.Membership.Entities.Dtos.UserRegistration
{
    public class UserRegistrationDto(string email, string password,
        string firstName, string lastName, string cedula)
    {
        public string Email => email;
        public string Password => password;
        public string FirstName => firstName;
        public string LastName => lastName;
        public string Cedula => cedula;
    }

}
