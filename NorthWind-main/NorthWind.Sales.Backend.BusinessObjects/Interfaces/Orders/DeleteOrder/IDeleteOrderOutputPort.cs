namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder
{
    public interface IDeleteOrderOutputPort
    {
        int OrderId { get; }
        Task Handle(int orderId);
    }
}
