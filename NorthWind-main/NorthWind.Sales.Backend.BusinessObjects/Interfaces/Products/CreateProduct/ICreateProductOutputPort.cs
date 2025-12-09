using NorthWind.Sales.Backend.BusinessObjects.Entities;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct
{
    /// <summary>
    /// OutputPort para el caso de uso "Crear Producto".
    /// Define el contrato que debe implementar el Presenter.
    /// </summary>
    public interface ICreateProductOutputPort
    {
        int ProductId { get; }
        Task Handle(Product product);
    }
}
