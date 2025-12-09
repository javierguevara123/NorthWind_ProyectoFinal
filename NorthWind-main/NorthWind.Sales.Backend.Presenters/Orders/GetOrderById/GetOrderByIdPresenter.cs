using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;

namespace NorthWind.Sales.Backend.Presenters.Orders.GetOrderById
{
    internal class GetOrderByIdPresenter : IGetOrderByIdOutputPort
    {
        public OrderWithDetailsDto? Order { get; private set; }

        public Task Handle(OrderWithDetailsDto? order)
        {
            Order = order;
            return Task.CompletedTask;
        }
    }
}
