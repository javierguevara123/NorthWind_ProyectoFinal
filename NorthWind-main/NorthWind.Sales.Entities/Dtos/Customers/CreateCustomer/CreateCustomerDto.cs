namespace NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer
{
    public class CreateCustomerDto(
        string id,
        string name,
        decimal currentBalance,
        string email,      // Nuevo
        string cedula,     // Nuevo
        string password)   // Nuevo (Texto plano)
    {
        public string Id => id;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
        public string Email => email;
        public string Cedula => cedula;
        public string Password => password;
    }
}