using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById
{
    public interface IGetCustomerByIdInputPort
    {
        Task Handle(GetCustomerByIdDto dto);
    }
}