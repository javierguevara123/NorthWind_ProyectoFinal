using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders
{
    public interface IGetOrdersInputPort
    {
        Task Handle(GetOrdersQueryDto query);
    }
}
