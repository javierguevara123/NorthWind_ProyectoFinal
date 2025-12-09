using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProductById;
using NorthWind.Sales.Entities.Dtos.Products.GetProductById;

namespace NorthWind.Sales.Backend.Presenters.Products.GetProductById
{
    /// <summary>
    /// Presenter para el caso de uso "Obtener Producto por ID".
    /// </summary>
    internal class GetProductByIdPresenter : IGetProductByIdOutputPort
    {
        public ProductDetailDto? Product { get; private set; }

        public Task Handle(ProductDetailDto? product)
        {
            Product = product;
            return Task.CompletedTask;
        }
    }
}
