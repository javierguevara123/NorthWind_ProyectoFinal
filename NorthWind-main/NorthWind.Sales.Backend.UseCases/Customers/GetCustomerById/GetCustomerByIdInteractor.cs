using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById; // Asegúrate de este using
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Customers.GetCustomerById
{
    internal class GetCustomerByIdInteractor(
        IGetCustomerByIdOutputPort outputPort,
        IQueriesRepository queriesCustomerRepository,
        IModelValidatorHub<GetCustomerByIdDto> modelValidatorHub,
        IUserService userService
    ) : IGetCustomerByIdInputPort
    {
        public async Task Handle(GetCustomerByIdDto dto)
        {
            // 1. Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // 2. Validar reglas de negocio
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // 3. Obtener el cliente (El repositorio devuelve CustomerDetailDto)
            var customer = await queriesCustomerRepository.GetCustomerById(dto.CustomerId);

            // 4. Enviar al output port
            await outputPort.Handle(customer);
        }
    }
}