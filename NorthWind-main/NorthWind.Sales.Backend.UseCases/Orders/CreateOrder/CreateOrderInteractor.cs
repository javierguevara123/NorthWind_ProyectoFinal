using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.ValueObjects;
using NorthWind.Entities.Guards;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.BusinessObjects.Aggregates;
using NorthWind.Sales.Backend.BusinessObjects.Guards;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.BusinessObjects.Specifications;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Exceptions.Entities.Exceptions;

namespace NorthWind.Sales.Backend.UseCases.Orders.CreateOrder;

// ************************************
// * InputPort                        *
// ************************************
// Función: Por medio del "Controller" el "InputPort" recibe los datos necesarios en el "Dto"
//          y los pasa al "Interactor" para que este pueda resolver el caso de uso "Crear orden".
//          Además le comunica al "Interactor" que luego de procesar el caso de uso NO DEBE
//          regresar NADA al "Controller".
// ************************************
// * OutputPort                       *
// ************************************
// Función: Una vez que el "Interactor" procesa-ejecuta el caso de uso "Crear orden"
//          el "OutputPort" le debe pasar al "Presenter" los datos que este debe
//          transformar-convertir y luego devolver al "Controller" para que algún agente
//          externo los utilice.

internal class CreateOrderInteractor(
    ICreateOrderOutputPort outputPort,
    ICommandsRepository repository,
    IModelValidatorHub<CreateOrderDto> modelValidatorHub,
    IDomainEventHub<SpecialOrderCreatedEvent> domainEventHub,
    IDomainLogger domainLogger,
    IDomainTransaction domainTransaction,
    IUserService userService) : ICreateOrderInputPort
{
    public async Task Handle(CreateOrderDto orderDto)
    {
        // 1. Validar autenticación
        GuardUser.AgainstUnauthenticated(userService);

        // 2. Validar modelo de entrada
        await GuardModel.AgainstNotValid(modelValidatorHub, orderDto);

        // ✅ CORRECCIÓN DE SEGURIDAD (Manejo de UserName nulo)
        // Intentamos obtener el nombre del usuario logueado.
        // Si es null (ej: Cliente JWT sin claim 'Name'), usamos el CustomerId del DTO.
        // Si aun así falla, usamos "Anonimo".
        string currentUserName = !string.IsNullOrEmpty(userService.UserName)
            ? userService.UserName
            : (orderDto.CustomerId ?? "Anonimo");

        // Log de inicio
        await domainLogger.LogInformation(new DomainLog(CreateOrderMessages.StartingPurchaseOrderCreation, currentUserName));

        // 3. Crear el Agregado
        OrderAggregate Order = OrderAggregate.From(orderDto);

        // Asignamos el EmployeeId con el usuario actual (o el CustomerId si es autopedido)
        Order.EmployeeId = currentUserName;

        try
        {
            // 4. INICIAR LA TRANSACCIÓN
            domainTransaction.BeginTransaction();

            // 5. CONCURRENCIA PESIMISTA: Obtener y Bloquear Productos (UPDLOCK)
            var productIds = Order.OrderDetails.Select(d => d.ProductId).ToList();
            var productsInDb = await repository.GetProductsWithLock(productIds);

            // 6. VALIDAR STOCK Y ACTUALIZAR EN MEMORIA
            foreach (var detail in Order.OrderDetails)
            {
                var product = productsInDb.FirstOrDefault(p => p.Id == detail.ProductId);

                // Validación: ¿Existe el producto?
                if (product == null)
                {
                    throw new ValidationException($"El producto con ID {detail.ProductId} no existe.");
                }

                // Validación: ¿Hay stock suficiente?
                if (product.UnitsInStock < detail.Quantity)
                {
                    throw new ValidationException($"Stock insuficiente para el producto '{product.Name}'. Stock actual: {product.UnitsInStock}, Solicitado: {detail.Quantity}");
                }

                // Lógica de Negocio: Restar Stock
                var nuevoStock = (short)(product.UnitsInStock - detail.Quantity);

                // 7. PREPARAR ACTUALIZACIÓN EN EL REPOSITORIO
                await repository.UpdateProductStock(product.Id, nuevoStock);
            }

            // 8. CREAR LA ORDEN (Prepara el INSERT en SQL)
            await repository.CreateOrder(Order);

            // 9. GUARDAR TODOS LOS CAMBIOS (Unit of Work)
            await repository.SaveChanges();

            // Log de éxito (Usando el nombre seguro)
            await domainLogger.LogInformation(new DomainLog(
                string.Format(CreateOrderMessages.PurchaseOrderCreatedTemplate, Order.Id),
                currentUserName));

            // 10. CONFIRMAR TRANSACCIÓN
            domainTransaction.CommitTransaction();

            // 11. NOTIFICAR ÉXITO AL PRESENTER
            await outputPort.Handle(Order);

            // 12. EVENTOS DE DOMINIO (Ej: Enviar correo si es orden especial)
            if (new SpecialOrderSpecification().IsSatisfiedBy(Order))
            {
                await domainEventHub.Raise(new SpecialOrderCreatedEvent(Order.Id, Order.OrderDetails.Count));
            }
        }
        catch (Exception ex)
        {
            // 13. MANEJO DE ERRORES (Rollback)
            domainTransaction.RollbackTransaction();

            string information = string.Format(CreateOrderMessages.OrderCreationCancelledTemplate, Order.Id);

            // Log de error (Usando el nombre seguro)
            await domainLogger.LogInformation(new DomainLog(
                $"{information}. Error: {ex.Message}",
                currentUserName));

            throw;
        }
    }
}