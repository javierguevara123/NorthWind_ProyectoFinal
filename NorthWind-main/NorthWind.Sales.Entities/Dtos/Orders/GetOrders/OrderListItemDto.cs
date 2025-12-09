namespace NorthWind.Sales.Entities.Dtos.Orders.GetOrders
{
    public record OrderListItemDto(
    int OrderId,
    string CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string ShipCity,
    string ShipCountry,
    decimal TotalAmount,
    int ItemCount
);
}
