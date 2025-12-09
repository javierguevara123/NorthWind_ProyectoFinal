using NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer
{   
    /// <summary>
    /// InputPort para el caso de uso "Actualizar Cliente".
    /// Define el contrato que debe implementar el Interactor.
    /// </summary>
    public interface IUpdateCustomerInputPort
    {
        Task Handle(UpdateCustomerDto dto);
    }
}
