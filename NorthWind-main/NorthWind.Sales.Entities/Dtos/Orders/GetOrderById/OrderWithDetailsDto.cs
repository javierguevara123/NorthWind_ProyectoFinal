namespace NorthWind.Sales.Entities.Dtos.Orders.GetOrderById
{
    /// <summary>
    /// DTO completo de una orden con todos sus campos y detalles.
    /// </summary>
    public record OrderWithDetailsDto(
        int OrderId,
        string CustomerId,
        string CustomerName,
        DateTime OrderDate,
        string ShipAddress,      // ← AGREGAR
        string ShipCity,
        string ShipCountry,
        string ShipPostalCode,   // ← AGREGAR
        decimal TotalAmount,
        int ItemCount,           // ← AGREGAR (opcional)
        IEnumerable<OrderDetailItemDto> OrderDetails
    );
}
