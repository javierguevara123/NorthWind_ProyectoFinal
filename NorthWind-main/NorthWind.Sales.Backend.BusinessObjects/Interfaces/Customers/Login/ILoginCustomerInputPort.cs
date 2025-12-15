using NorthWind.Sales.Entities.Dtos.Customers.Login;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.Login
{
    public interface ILoginCustomerInputPort
    {
        Task<LoginResponseDto> Handle(LoginCustomerDto dto);
    }
}
