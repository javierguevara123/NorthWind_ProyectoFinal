using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Customers.DeleteCustomer
{
    internal class DeleteCustomerInteractor(
        IDeleteCustomerOutputPort outputPort,
        ICommandsRepository commandsRepository,
        IModelValidatorHub<DeleteCustomerDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : IDeleteCustomerInputPort
    {
        public async Task Handle(DeleteCustomerDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar reglas de negocio
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Log de inicio
            await domainLogger.LogInformation(
                new DomainLog(
                    DeleteCustomerMessages.StartingCustomerDeletion,
                    userService.UserName));

            try
            {
                // 4. Iniciar transacción
                domainTransaction.BeginTransaction();

                // 5. Eliminar cliente
                await commandsRepository.DeleteCustomer(dto.CustomerId);

                // 6. Guardar cambios
                await commandsRepository.SaveChanges();

                // 7. Log de éxito
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            DeleteCustomerMessages.CustomerDeletedTemplate,
                            dto.CustomerId),
                        userService.UserName));

                // 8. Enviar resultado al presentador
                await outputPort.Handle(dto.CustomerId);

                // 9. Confirmar transacción
                domainTransaction.CommitTransaction();
            }
            catch
            {
                // Rollback
                domainTransaction.RollbackTransaction();

                // Log del fallo
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            DeleteCustomerMessages.CustomerDeletionCancelledTemplate,
                            dto.CustomerId),
                        userService.UserName));

                throw;
            }
        }
    }
}
