using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct
{
    /// <summary>
    /// InputPort para el caso de uso "Crear Producto".
    /// Define el contrato que debe implementar el Interactor.
    /// </summary>
    public interface ICreateProductInputPort
    {
        Task Handle(CreateProductDto dto);
    }
}
