namespace NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer
{
    public class UpdateCustomerDto(
        string customerid,
        string name,
        decimal currentBalance,
        string email,      // Nuevo
        string cedula)     // Nuevo
    {
        public string CustomerId => customerid;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
        public string Email => email;
        public string Cedula => cedula;
    }
}