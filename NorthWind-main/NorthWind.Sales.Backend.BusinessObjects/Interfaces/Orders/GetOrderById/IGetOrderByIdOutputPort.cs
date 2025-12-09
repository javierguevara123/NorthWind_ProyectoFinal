using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById
{
    public interface IGetOrderByIdOutputPort
    {
        OrderWithDetailsDto? Order { get; }
        Task Handle(OrderWithDetailsDto? order);
    }
}
