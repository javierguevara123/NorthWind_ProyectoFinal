using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Orders.GetOrders
{
    internal class GetOrdersInteractor(
        IGetOrdersOutputPort outputPort,
        IQueriesRepository queriesRepository,
        IModelValidatorHub<GetOrdersQueryDto> modelValidatorHub,
        IUserService userService) : IGetOrdersInputPort
    {
        public async Task Handle(GetOrdersQueryDto query)
        {
            GuardUser.AgainstUnauthenticated(userService);
            await GuardModel.AgainstNotValid(modelValidatorHub, query);

            var result = await queriesRepository.GetOrdersPaged(query);
            await outputPort.Handle(result);
        }
    }
}
