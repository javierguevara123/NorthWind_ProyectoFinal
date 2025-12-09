namespace NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById
{
    public record CustomerDetailDto(
        string Id,
        string Name,
        decimal CurrentBalance
    );
}
