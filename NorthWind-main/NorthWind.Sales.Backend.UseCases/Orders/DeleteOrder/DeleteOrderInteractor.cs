using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Orders.DeleteOrder;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Orders.DeleteOrder
{
    internal class DeleteOrderInteractor(
        IDeleteOrderOutputPort outputPort,
        ICommandsRepository commandsRepository,
        IModelValidatorHub<DeleteOrderDto> modelValidatorHub,
        IDomainLogger domainLogger,
        IDomainTransaction domainTransaction,
        IUserService userService) : IDeleteOrderInputPort
    {
        public async Task Handle(DeleteOrderDto dto)
        {
            GuardUser.AgainstUnauthenticated(userService);
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            await domainLogger.LogInformation(
                new DomainLog($"Iniciando eliminación de orden {dto.OrderId}", userService.UserName));

            try
            {
                domainTransaction.BeginTransaction();

                await commandsRepository.DeleteOrder(dto.OrderId);
                await commandsRepository.SaveChanges();

                await domainLogger.LogInformation(
                    new DomainLog($"Orden {dto.OrderId} eliminada exitosamente", userService.UserName));

                await outputPort.Handle(dto.OrderId);

                domainTransaction.CommitTransaction();
            }
            catch
            {
                domainTransaction.RollbackTransaction();
                await domainLogger.LogInformation(
                    new DomainLog($"Eliminación de orden {dto.OrderId} cancelada", userService.UserName));
                throw;
            }
        }
    }
}
