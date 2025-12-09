
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.UseCases.Products.GetProducts
{
    /// <summary>
    /// Interactor para el caso de uso "Obtener Productos".
    /// Recupera productos con paginación y filtros.
    /// </summary>
    internal class GetProductsInteractor(
        IGetProductsOutputPort outputPort,
        IQueriesRepository repository) : IGetProductsInputPort
    {
        public async Task Handle(GetProductsQueryDto query)
        {
            // 1. Validar parámetros de consulta
            query.Validate();

            // 2. Obtener productos con paginación
            var result = await repository.GetProductsPaged(query);

            // 3. Enviar resultado al OutputPort (Presenter)
            await outputPort.Handle(result);
        }
    }
}
