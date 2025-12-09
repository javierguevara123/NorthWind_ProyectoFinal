using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;

namespace NorthWind.Sales.Backend.Presenters.Customers.GetCustomers
{
    internal class GetCustomersPresenter : IGetCustomersOutputPort
    {
        // Propiedad pública de solo lectura que el controller va a leer.
        public CustomerPagedResultDto? Result { get; private set; }

        // El interactor llamará a este método con el resultado.
        public Task Handle(CustomerPagedResultDto result)
        {
            Result = result;
            return Task.CompletedTask;
        }
    }
}

