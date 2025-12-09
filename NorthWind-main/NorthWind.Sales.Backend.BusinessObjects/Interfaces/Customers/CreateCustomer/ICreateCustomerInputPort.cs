using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer
{
    public interface ICreateCustomerInputPort
    {
        Task Handle(CreateCustomerDto dto);
    }
}