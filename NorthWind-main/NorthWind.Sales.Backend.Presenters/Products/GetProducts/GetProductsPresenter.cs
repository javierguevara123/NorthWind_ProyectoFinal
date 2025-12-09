using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProducts;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.Presenters.Products.GetProducts
{
    /// <summary>
    /// Presenter para el caso de uso "Obtener Productos".
    /// Formatea la respuesta que se enviará al Controller.
    /// </summary>
    internal class GetProductsPresenter : IGetProductsOutputPort
    {
        public PagedResultDto<ProductDto> Result { get; private set; } = new();

        public Task Handle(PagedResultDto<ProductDto> result)
        {
            Result = result;
            return Task.CompletedTask;
        }
    }
}
