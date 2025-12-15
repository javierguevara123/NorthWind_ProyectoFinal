namespace NorthWind.Sales.Entities.Dtos.Customers.Login
{
    public class LoginResponseDto(string token, string customerId, string name)
    {
        public string Token => token;
        public string CustomerId => customerId;
        public string Name => name;
    }
}
