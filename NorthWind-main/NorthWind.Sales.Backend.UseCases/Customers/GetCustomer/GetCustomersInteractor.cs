
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;

namespace NorthWind.Sales.Backend.UseCases.Customers.GetCustomers
{
    internal class GetCustomersInteractor(
        IGetCustomersOutputPort outputPort,
        IQueriesRepository repository)
        : IGetCustomersInputPort
    {
        public async Task Handle(GetCustomersQueryDto query)
        {
            // 1. Validar parámetros
            query.Validate();

            // 2. Obtener clientes paginados
            var result = await repository.GetCustomersPaged(query);

            // 3. Enviar resultado al output port
            await outputPort.Handle(result);
        }
    }
}
