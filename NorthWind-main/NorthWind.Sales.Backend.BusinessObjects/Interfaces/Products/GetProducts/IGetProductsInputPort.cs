using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProducts;

/// <summary>
/// InputPort para el caso de uso "Obtener Productos".
/// Define el contrato que debe implementar el Interactor.
/// </summary>
public interface IGetProductsInputPort
{
    Task Handle(GetProductsQueryDto query);
}
