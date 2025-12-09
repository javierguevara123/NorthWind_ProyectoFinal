using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById 
{
    public interface IGetCustomerByIdOutputPort
    {
        CustomerDetailDto? Customer { get; }
       
        Task Handle(CustomerDetailDto customer);
    }
}
