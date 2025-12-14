using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Entities;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Backend.UseCases.Products.CreateProduct
{
    internal class CreateProductInteractor(
    ICreateProductOutputPort outputPort,
    ICommandsRepository repository,
    IModelValidatorHub<CreateProductDto> modelValidatorHub,
    IDomainLogger domainLogger,
    IDomainTransaction domainTransaction,
    IUserService userService) : ICreateProductInputPort
    {
        public async Task Handle(CreateProductDto dto)
        {
            GuardUser.AgainstUnauthenticated(userService);
            await GuardModel.AgainstNotValid(modelValidatorHub, dto);

            await domainLogger.LogInformation(
                new DomainLog(
                    CreateProductMessages.StartingProductCreation,
                    userService.UserName));

            // 3. Lógica de Conversión de Imagen (Base64 -> Bytes)
            byte[]? imageBytes = null;
            if (!string.IsNullOrEmpty(dto.ProfilePictureBase64))
            {
                try
                {
                    // Limpiar encabezado si viene (data:image/png;base64,...)
                    var base64Clean = dto.ProfilePictureBase64.Contains(",")
                        ? dto.ProfilePictureBase64.Split(',')[1]
                        : dto.ProfilePictureBase64;

                    imageBytes = Convert.FromBase64String(base64Clean);
                }
                catch
                {
                    // Si falla, guardamos null
                    imageBytes = null;
                }
            }

            // ⬅️ Crea Product de DOMINIO (Capa 2)
            var product = new Product
            {
                Name = dto.Name,
                UnitsInStock = dto.UnitsInStock,
                UnitPrice = dto.UnitPrice,
                ProfilePicture = imageBytes
            };

            try
            {
                domainTransaction.BeginTransaction();

                // ⬅️ Repository mapea internamente Dominio → Persistencia
                int generatedId = await repository.CreateProduct(product);
                product.Id = generatedId; // ⬅️ Actualiza manualmente
                await repository.SaveChanges();

                await domainLogger.LogInformation(
                    new DomainLog(
                        string.Format(
                            CreateProductMessages.ProductCreatedTemplate,
                            product.Id),
                        userService.UserName));

                // ⬅️ Envía Product de DOMINIO al OutputPort
                await outputPort.Handle(product);

                domainTransaction.CommitTransaction();
            }
            catch
            {
                domainTransaction.RollbackTransaction();

                string information = string.Format(
                    CreateProductMessages.ProductCreationCancelledTemplate,
                    product.Id);

                await domainLogger.LogInformation(
                    new DomainLog(information, userService.UserName));

                throw;
            }
        }
    }
}
