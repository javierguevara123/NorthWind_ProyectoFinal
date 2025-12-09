namespace NorthWind.Sales.Entities.Dtos.Orders.GetOrderById
{
    /// <summary>
    /// DTO para representar un detalle de orden.
    /// </summary>
    public record OrderDetailItemDto(
        int ProductId,
        string ProductName,
        short Quantity,
        decimal UnitPrice,
        decimal Subtotal
    );
}
