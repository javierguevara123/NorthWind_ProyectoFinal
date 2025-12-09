using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;

namespace NorthWind.Sales.Backend.Presenters.Orders.GetOrders
{
    internal class GetOrdersPresenter : IGetOrdersOutputPort
    {
        public OrderPagedResultDto Result { get; private set; } = new();

        public Task Handle(OrderPagedResultDto result)
        {
            Result = result;
            return Task.CompletedTask;
        }
    }
}
