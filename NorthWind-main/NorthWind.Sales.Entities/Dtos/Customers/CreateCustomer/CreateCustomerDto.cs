namespace NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer
{
    public class CreateCustomerDto(
        string id, 
        string name, 
        decimal currentBalance)
    {
        public string Id => id;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
    }
}