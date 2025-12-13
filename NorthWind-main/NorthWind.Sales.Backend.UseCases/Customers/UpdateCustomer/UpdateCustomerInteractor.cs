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

namespace NorthWind.Sales.Backend.UseCases.Customers.UpdateCustomer
{
    internal class UpdateCustomerInteractor(
        IUpdateCustomerOutputPort outputPort,
        ICommandsRepository commandsRepository,
        IQueriesRepository queriesRepository,
        IModelValidatorHub<UpdateCustomerDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : IUpdateCustomerInputPort
    {
        public async Task Handle(UpdateCustomerDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar modelo
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Log inicial
            await domainLogger.LogInformation(
                new DomainLog(
                    UpdateCustomerMessages.StartingCustomerUpdate,
                    userService.UserName));

            // 4. Mapear DTO a entidad Customer
            // NOTA: No mapeamos HashedPassword aquí para evitar sobrescribirla con vacío
            var customer = new Customer
            {
                Id = dto.CustomerId,
                Name = dto.Name,
                CurrentBalance = dto.CurrentBalance,
                Email = dto.Email,
                Cedula = dto.Cedula
            };

            try
            {
                domainTransaction.BeginTransaction();

                // 6. Actualizar cliente (El repositorio debe encargarse de no borrar la pass)
                await commandsRepository.UpdateCustomer(customer);

                // 7. Guardar cambios
                await commandsRepository.SaveChanges();

                // 8. Log de éxito
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            UpdateCustomerMessages.CustomerUpdatedTemplate,
                            customer.Id),
                        userService.UserName));

                // 9. Enviar respuesta
                await outputPort.Handle(customer);

                domainTransaction.CommitTransaction();
            }
            catch
            {
                domainTransaction.RollbackTransaction();
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            UpdateCustomerMessages.CustomerUpdateCancelledTemplate,
                            customer.Id),
                        userService.UserName));
                throw;
            }
        }
    }
}