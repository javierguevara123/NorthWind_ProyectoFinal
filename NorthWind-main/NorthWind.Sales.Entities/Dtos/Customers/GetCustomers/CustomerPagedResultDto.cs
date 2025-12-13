namespace NorthWind.Sales.Entities.Dtos.Customers.GetCustomers
{
    public class CustomerPagedResultDto
    {
        public IEnumerable<CustomerListItemDto> Customers { get; }
        public int TotalRecords { get; }

        public CustomerPagedResultDto(IEnumerable<CustomerListItemDto> customers, int totalRecords)
        {
            Customers = customers;
            TotalRecords = totalRecords;
        }
    }

    public class CustomerListItemDto(
        string id,
        string name,
        decimal currentBalance,
        string email,    // <--- NUEVO
        string cedula)   // <--- NUEVO
    {
        public string Id => id;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
        public string Email => email;   // <--- NUEVO
        public string Cedula => cedula; // <--- NUEVO
    }
}
