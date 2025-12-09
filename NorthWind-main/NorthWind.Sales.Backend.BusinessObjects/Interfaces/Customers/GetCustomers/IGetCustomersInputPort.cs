

using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers
{
    /// <summary>
    /// InputPort para el caso de uso "Obtener Clientes".
    /// Define el contrato que debe implementar el Interactor.
    /// </summary>
    public interface IGetCustomersInputPort
    {
        Task Handle(GetCustomersQueryDto query);
    }
}
