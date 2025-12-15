namespace NorthWind.Sales.Entities.Dtos.Customers.Login
{
    public class LoginCustomerDto(string email, string password)
    {
        public string Email => email;
        public string Password => password;
    }
}
