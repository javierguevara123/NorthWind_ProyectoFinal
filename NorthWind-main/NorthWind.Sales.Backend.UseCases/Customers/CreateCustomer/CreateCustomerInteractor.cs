using System.Security.Cryptography;
using System.Text;
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
            // 1. OMITIR validación de autenticación (Para permitir registro público)
            // GuardUser.AgainstUnauthenticated(userService);

            // 2. Validación del Modelo
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // ✅ CORRECCIÓN: Definir usuario seguro para el Log (evita el error NULL en SQL)
            // Si el usuario no está logueado (registro), usamos su nombre o "Guest".
            string logUser = !string.IsNullOrWhiteSpace(userService.UserName)
                ? userService.UserName
                : (dto.Email ?? "Guest");

            // 3. Log inicial usando la variable segura 'logUser'
            await domainLogger.LogInformation(
                new DomainLog(
                    CreateCustomerMessages.StartingCustomerCreation,
                    logUser)); // 👈 Cambio aquí

            // 4. Hashear contraseña
            string hashedPassword = HashPassword(dto.Password);

            // 5. Procesar Imagen
            byte[]? imageBytes = null;
            if (!string.IsNullOrEmpty(dto.ProfilePictureBase64))
            {
                try
                {
                    var base64Clean = dto.ProfilePictureBase64.Contains(",")
                        ? dto.ProfilePictureBase64.Split(',')[1]
                        : dto.ProfilePictureBase64;
                    imageBytes = Convert.FromBase64String(base64Clean);
                }
                catch { imageBytes = null; }
            }

            // 6. Crear Entidad
            var customer = new Customer
            {
                Id = dto.Id,
                Name = dto.Name,
                CurrentBalance = dto.CurrentBalance,
                Email = dto.Email,
                Cedula = dto.Cedula,
                HashedPassword = hashedPassword,
                ProfilePicture = imageBytes
            };

            try
            {
                domainTransaction.BeginTransaction();

                string generatedId = await repository.CreateCustomer(customer);
                customer.Id = generatedId;

                await repository.SaveChanges();

                // 7. Log Final (Usando también 'logUser')
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            CreateCustomerMessages.CustomerCreatedTemplate,
                            customer.Id),
                        logUser)); // 👈 Cambio aquí

                await outputPort.Handle(customer.Id);

                domainTransaction.CommitTransaction();
            }
            catch
            {
                domainTransaction.RollbackTransaction();

                // Log de error (Usando también 'logUser')
                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            CreateCustomerMessages.CustomerCreationCancelledTemplate,
                            customer.Id),
                        logUser)); // 👈 Cambio aquí

                throw;
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}