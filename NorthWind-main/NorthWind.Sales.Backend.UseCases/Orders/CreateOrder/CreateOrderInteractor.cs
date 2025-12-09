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
//
//
//  Por lo tanto el "Interactor" que "necesita" para realizar su trabajo:
//  1).- Utiliza o necesita de un "InputPort" porque este le pasa los datos (Dto) y debe implentar
//       los métodos de esta "Interface" para que este pueda ejecutar el caso de uso "Crear orden".
//  2).- Utiliza o necesita de un repositorio para realizar la lógica de la persistencia de datos.
//  3).  Utiliza o necesita de un "OutputPort" porque por medio o a través del "OutputPort" debe
//       regresar el resultadoo o datos de salida al "Presenter" una vez ejecutada la lógica del
//       caso de uso "Crear orden".
//
//  RESUMEN: El "Interactor" no sabe que clase lo va a utilizar, su función es:
//           1).- Procesar el caso de uso "Crear orden" con los datos que le pasa el "InputPort".
//           2).- Realizar la persistencia de los datos para lo cual necesita de un "Repository" y.
//           3).- Regresar el resultado del caso de uso al "OutputPort", el cual debe pasar los
//                datos al "Presenter", datos que este debe transformar-convertir y luego devolver
//                al "Controller" para que algún agente externo los utilice.
//
//  NOTA: Objetos necesarios:
//        1).- Un "InputPort" que tiene los datos, además esta interfaz tiene el o los métodos
//             que el "Interactor" debe implementar para procesar el caso de uso "Crear orden"
//             esto se muestra en el método "Handle(CreateOrderDto orderDto)".
//        2).- Un "outputPort" y un "repository" los cuales se los pasa en el "Constructor"
//             mediante la técnica-mecanismo de "Inyección de dependencias a través del
//             constructor", "outputPort, repository".

internal class CreateOrderInteractor(ICreateOrderOutputPort outputPort,
                                     ICommandsRepository repository,
                                     IModelValidatorHub<CreateOrderDto> modelValidatorHub,
                                     IDomainEventHub<SpecialOrderCreatedEvent> domainEventHub,
                                     IDomainLogger domainLogger,
                                     IDomainTransaction domainTransaction,
                                     IUserService userService) : ICreateOrderInputPort
{
    public async Task Handle(CreateOrderDto orderDto)
    {
        GuardUser.AgainstUnauthenticated(userService);
        await GuardModel.AgainstNotValid(modelValidatorHub, orderDto);
        await domainLogger.LogInformation(new DomainLog(CreateOrderMessages.StartingPurchaseOrderCreation, userService.UserName));

        OrderAggregate Order = OrderAggregate.From(orderDto);

        Order.EmployeeId = userService.UserName;

        try
        {
            // 1. INICIAR LA TRANSACCIÓN
            domainTransaction.BeginTransaction();

            // 2. CONCURRENCIA PESIMISTA: Obtener y Bloquear Productos (UPDLOCK)
            // Esto asegura que nadie más pueda modificar estos productos mientras validamos y actualizamos.
            var productIds = Order.OrderDetails.Select(d => d.ProductId).ToList();
            var productsInDb = await repository.GetProductsWithLock(productIds);

            // 3. VALIDAR STOCK Y ACTUALIZAR EN MEMORIA
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

                // 4. PREPARAR ACTUALIZACIÓN EN EL REPOSITORIO
                // Esto prepara el UPDATE en el contexto de EF Core
                await repository.UpdateProductStock(product.Id, nuevoStock);
            }

            // 5. CREAR LA ORDEN (Prepara el INSERT en SQL)
            await repository.CreateOrder(Order);

            // 6. GUARDAR TODOS LOS CAMBIOS (Unit of Work)
            // Aquí se ejecutan los UPDATE de productos y el INSERT de la orden en una sola ida a la BD
            await repository.SaveChanges();

            await domainLogger.LogInformation(new DomainLog(string.Format(
                CreateOrderMessages.PurchaseOrderCreatedTemplate, Order.Id), userService.UserName));

            // 7. CONFIRMAR TRANSACCIÓN (Libera los bloqueos UPDLOCK)
            domainTransaction.CommitTransaction();

            // 8. NOTIFICAR ÉXITO
            await outputPort.Handle(Order);

            if (new SpecialOrderSpecification().IsSatisfiedBy(Order))
            {
                await domainEventHub.Raise(new SpecialOrderCreatedEvent(Order.Id, Order.OrderDetails.Count));
            }
        }
        catch (Exception ex)
        {
            // 9. SI ALGO FALLA, DESHACER TODO
            domainTransaction.RollbackTransaction();

            string Information = string.Format(CreateOrderMessages.OrderCreationCancelledTemplate, Order.Id);
            // Loguear el error real para depuración
            await domainLogger.LogInformation(new DomainLog($"{Information}. Error: {ex.Message}", userService.UserName));
            throw;
        }
    }
}