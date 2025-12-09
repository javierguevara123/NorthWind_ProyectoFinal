using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders
{
    public interface IGetOrdersOutputPort
    {
        OrderPagedResultDto Result { get; }
        Task Handle(OrderPagedResultDto result);
    }
}
