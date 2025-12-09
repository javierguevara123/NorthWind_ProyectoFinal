using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Entities;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Products.UpdateProduct
{
    /// <summary>
    /// Interactor para el caso de uso "Actualizar Producto".
    /// Contiene la lógica de negocio para actualizar un producto existente.
    /// </summary>
    internal class UpdateProductInteractor(
        IUpdateProductOutputPort outputPort,
        ICommandsRepository commandsRepository,
        IQueriesRepository queriesRepository,
        IModelValidatorHub<UpdateProductDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : IUpdateProductInputPort
    {
        public async Task Handle(UpdateProductDto dto)
        {
            // 1. Validar que el usuario esté autenticado
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar el modelo (DTO)
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Registrar log de inicio
            await domainLogger.LogInformation(
                new DomainLog(
                    UpdateProductMessages.StartingProductUpdate,
                    userService.UserName));

            // 4. Mapear DTO a entidad Product (POCO)
            var product = new Product
            {
                Id = dto.ProductId,
                Name = dto.Name,
                UnitsInStock = dto.UnitsInStock,
                UnitPrice = dto.UnitPrice,
                //LastModifiedDate = DateTime.Now  // Actualizar fecha de modificación
            };

            try
            {
                // 5. Iniciar transacción
                domainTransaction.BeginTransaction();

                // 6. Actualizar el producto
                await commandsRepository.UpdateProduct(product);

                // 7. Confirmar cambios en la base de datos (Unit of Work)
                await commandsRepository.SaveChanges();

                // 8. Registrar log de éxito
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            UpdateProductMessages.ProductUpdatedTemplate,
                            product.Id),
                        userService.UserName));

                // 9. Enviar respuesta al OutputPort (Presenter)
                await outputPort.Handle(product);

                // 10. Confirmar la transacción
                domainTransaction.CommitTransaction();
            }
            catch
            {
                // 11. Cancelar la transacción en caso de error
                domainTransaction.RollbackTransaction();

                // 12. Registrar log de cancelación
                string information = string.Format(
                    UpdateProductMessages.ProductUpdateCancelledTemplate,
                    product.Id);

                await domainLogger.LogInformation(
                    new DomainLog(information, userService.UserName));

                throw;
            }
        }
    }
}
