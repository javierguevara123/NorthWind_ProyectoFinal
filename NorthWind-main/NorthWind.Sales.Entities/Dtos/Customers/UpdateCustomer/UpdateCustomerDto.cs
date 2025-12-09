namespace NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer
{
    public class UpdateCustomerDto(
        string customerid,
        string name,
        decimal currentBalance)
    {
        public string CustomerId => customerid;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
    }
}