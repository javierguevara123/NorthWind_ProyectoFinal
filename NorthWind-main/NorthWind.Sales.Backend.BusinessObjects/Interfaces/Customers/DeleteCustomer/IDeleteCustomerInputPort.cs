using NorthWind.Sales.Entities.Dtos.Customers.DeleteCustomer;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer
{
    public interface IDeleteCustomerInputPort
    {
        Task Handle(DeleteCustomerDto dto);
    }
}