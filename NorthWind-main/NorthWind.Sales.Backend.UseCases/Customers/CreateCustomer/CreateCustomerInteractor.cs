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

namespace NorthWind.Sales.Backend.UseCases.Customers.CreateCustomer
{
    internal class CreateCustomerInteractor(
        ICreateCustomerOutputPort outputPort,
        ICommandsRepository repository,
        IModelValidatorHub<CreateCustomerDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : ICreateCustomerInputPort
    {
        public async Task Handle(CreateCustomerDto dto)
        {
            // ✔ Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // ✔ Validación del DTO
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // ✔ Log inicial
            await domainLogger.LogInformation(
                new DomainLog(
                    CreateCustomerMessages.StartingCustomerCreation,
                    userService.UserName));

            // ✔ Crear entidad de dominio
            var customer = new Customer
            {
                Id = dto.Id,
                Name = dto.Name,
                CurrentBalance = dto.CurrentBalance
            };

            try
            {
                domainTransaction.BeginTransaction();

                // ✔ Guardar
                string generatedId = await repository.CreateCustomer(customer);
                customer.Id = generatedId;

                await repository.SaveChanges();

                // ✔ Log final
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            CreateCustomerMessages.CustomerCreatedTemplate,
                            customer.Id),
                        userService.UserName));

                // ✔ Enviar al Presenter
                await outputPort.Handle(customer.Id);

                domainTransaction.CommitTransaction();
            }
            catch
            {
                domainTransaction.RollbackTransaction();

                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            CreateCustomerMessages.CustomerCreationCancelledTemplate,
                            customer.Id),
                        userService.UserName));

                throw;
            }
        }
    }
}
