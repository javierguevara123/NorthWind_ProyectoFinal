using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Products.DeleteProduct
{
    /// <summary>
    /// Interactor para el caso de uso "Eliminar Producto".
    /// </summary>
    internal class DeleteProductInteractor(
        IDeleteProductOutputPort outputPort,
        ICommandsRepository commandsRepository,
        IModelValidatorHub<DeleteProductDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : IDeleteProductInputPort
    {
        public async Task Handle(DeleteProductDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar modelo (incluye verificar que existe y no tiene órdenes pendientes)
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Log de inicio
            await domainLogger.LogInformation(
                new DomainLog(
                    DeleteProductMessages.StartingProductDeletion,
                    userService.UserName));

            try
            {
                // 4. Iniciar transacción
                domainTransaction.BeginTransaction();

                // 5. Eliminar producto
                await commandsRepository.DeleteProduct(dto.ProductId);

                // 6. Persistir cambios
                await commandsRepository.SaveChanges();

                // 7. Log de éxito
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            DeleteProductMessages.ProductDeletedTemplate,
                            dto.ProductId),
                        userService.UserName));

                // 8. Enviar resultado al Presenter
                await outputPort.Handle(dto.ProductId);

                // 9. Confirmar transacción
                domainTransaction.CommitTransaction();
            }
            catch
            {
                // 10. Rollback en caso de error
                domainTransaction.RollbackTransaction();

                // 11. Log de cancelación
                string information = string.Format(
                    DeleteProductMessages.ProductDeletionCancelledTemplate,
                    dto.ProductId);

                await domainLogger.LogInformation(
                    new DomainLog(information, userService.UserName));

                throw;
            }
        }
    }
}
