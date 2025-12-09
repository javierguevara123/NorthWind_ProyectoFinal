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

    public record CustomerListItemDto(
        string Id,
        string Name,
        decimal CurrentBalance
    );
}
