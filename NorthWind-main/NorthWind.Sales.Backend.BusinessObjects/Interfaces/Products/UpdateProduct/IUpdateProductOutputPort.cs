using NorthWind.Sales.Backend.BusinessObjects.Entities;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.UpdateProduct
{
    /// <summary>
    /// OutputPort para el caso de uso "Actualizar Producto".
    /// Define el contrato que debe implementar el Presenter.
    /// </summary>
    public interface IUpdateProductOutputPort
    {
        int ProductId { get; }
        Product? Product { get; }
        Task Handle(Product product);
    }
}
