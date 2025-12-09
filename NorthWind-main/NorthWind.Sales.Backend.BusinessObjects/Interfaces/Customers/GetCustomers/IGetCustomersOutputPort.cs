namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers
{
    using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;
    using System.Threading.Tasks;

    public interface IGetCustomersOutputPort
    {
        /// <summary>
        /// Resultado que será leído por el controller después de que el interactor llame a Handle.
        /// </summary>
        CustomerPagedResultDto? Result { get; }

        /// <summary>
        /// El interactor llama a este método para pasar el resultado al presenter.
        /// </summary>
        Task Handle(CustomerPagedResultDto result);
    }
}
