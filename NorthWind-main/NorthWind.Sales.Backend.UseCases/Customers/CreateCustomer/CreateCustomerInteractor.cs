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
            // ✔ Validar autenticación
            GuardUser.AgainstUnauthenticated(userService);

            // ✔ Validación del DTO
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            // ✔ Log inicial
            await domainLogger.LogInformation(
                new DomainLog(
                    CreateCustomerMessages.StartingCustomerCreation,
                    userService.UserName));

            // ✔ Hashear contraseña (Implementación simple con SHA256)
            string hashedPassword = HashPassword(dto.Password);

            byte[]? imageBytes = null;

            if (!string.IsNullOrEmpty(dto.ProfilePictureBase64))
            {
                try
                {
                    // Limpiamos el header del base64 si viene del front (ej: "data:image/png;base64,")
                    var base64Clean = dto.ProfilePictureBase64;
                    if (base64Clean.Contains(","))
                    {
                        base64Clean = base64Clean.Split(',')[1];
                    }

                    imageBytes = Convert.FromBase64String(base64Clean);
                }
                catch
                {
                    // Si el string no es válido, lo dejamos null o lanzamos error
                    imageBytes = null;
                }
            }

            // ✔ Crear entidad de dominio
            var customer = new Customer
            {
                Id = dto.Id,
                Name = dto.Name,
                CurrentBalance = dto.CurrentBalance,
                Email = dto.Email,             // Nuevo
                Cedula = dto.Cedula,           // Nuevo
                HashedPassword = hashedPassword,
                ProfilePicture = imageBytes
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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}