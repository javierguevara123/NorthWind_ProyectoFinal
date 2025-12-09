using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Products.GetProductById
{
    /// <summary>
    /// Interactor para el caso de uso "Obtener Producto por ID".
    /// </summary>
    internal class GetProductByIdInteractor(
        IGetProductByIdOutputPort outputPort,
        IQueriesRepository queriesRepository,
        IModelValidatorHub<GetProductByIdDto> modelValidatorHub,
        IUserService userService) : IGetProductByIdInputPort
    {
        public async Task Handle(GetProductByIdDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar modelo
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Consultar el producto
            var product = await queriesRepository.GetProductById(dto.ProductId);

            // 4. Mapear a ProductDetailDto
            ProductDetailDto? productDetail = product != null
                ? new ProductDetailDto(
                    product.Id,
                    product.Name,
                    product.UnitsInStock,
                    product.UnitPrice)
                : null;

            // 5. Enviar resultado al Presenter
            await outputPort.Handle(productDetail);
        }
    }
}
