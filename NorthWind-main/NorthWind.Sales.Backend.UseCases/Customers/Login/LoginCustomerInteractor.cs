using System.Security.Cryptography;
using System.Text;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Common;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.Login;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Customers.Login;

namespace NorthWind.Sales.Backend.UseCases.Customers.Login
{
    

    internal class LoginCustomerInteractor(
        IQueriesRepository repository,
        IJwtService jwtService) : ILoginCustomerInputPort
    {
        public async Task<LoginResponseDto> Handle(LoginCustomerDto dto)
        {
            // 1. Buscar cliente por email
            var customer = await repository.GetCustomerCredentialsByEmail(dto.Email);

            if (customer == null)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            // 2. Verificar Hash (Usando el mismo SHA256 que en Create/Update)
            string inputHash = HashPassword(dto.Password);

            if (inputHash != customer.HashedPassword)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            // 3. Generar JWT
            string token = jwtService.GenerateToken(customer.Id, customer.Email, customer.Name);

            return new LoginResponseDto(token, customer.Id, customer.Name);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}