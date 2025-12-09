using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Orders.GetOrderById
{
    internal class GetOrderByIdInteractor(
        IGetOrderByIdOutputPort outputPort,
        IQueriesRepository queriesRepository,
        IModelValidatorHub<GetOrderByIdDto> modelValidatorHub,
        IUserService userService) : IGetOrderByIdInputPort
    {
        public async Task Handle(GetOrderByIdDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar modelo
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Consultar la orden con sus detalles
            var order = await queriesRepository.GetOrderById(dto.OrderId);

            // 4. Enviar resultado al Presenter
            await outputPort.Handle(order);
        }
    }
}
