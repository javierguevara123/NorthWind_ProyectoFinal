using NorthWind.Sales.Backend.BusinessObjects.Entities;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;

namespace NorthWind.Sales.Backend.PresentersCustomer.GetCustomerById
{
    
    /// <summary>
    /// Presenter para el caso de uso "Obtener Cliente por ID".
    /// </summary>
    internal class GetCustomerByIdPresenter : IGetCustomerByIdOutputPort
    {
        public CustomerDetailDto? Customer { get; private set; }
        public Task Handle(CustomerDetailDto? customer)
        {
            Customer = customer;
            return Task.CompletedTask;
        }
    }
}