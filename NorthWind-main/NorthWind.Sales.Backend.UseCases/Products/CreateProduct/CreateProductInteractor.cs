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

            // ⬅️ Crea Product de DOMINIO (Capa 2)
            var product = new Product
            {
                Name = dto.Name,
                UnitsInStock = dto.UnitsInStock,
                UnitPrice = dto.UnitPrice
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
