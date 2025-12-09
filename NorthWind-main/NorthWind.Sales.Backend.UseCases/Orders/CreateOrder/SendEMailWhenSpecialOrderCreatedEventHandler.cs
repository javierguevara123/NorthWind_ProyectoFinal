using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.UseCases.Resources;

namespace NorthWind.Sales.Backend.UseCases.Orders.CreateOrder
{
    internal class SendEMailWhenSpecialOrderCreatedEventHandler(IMailService mailService) : IDomainEventHandler<SpecialOrderCreatedEvent>
    {
        public Task Handle(SpecialOrderCreatedEvent createdOrder)
        {
            return mailService.SendMailToAdministrator(
            CreateOrderMessages.SendEmailSubject,
            string.Format(CreateOrderMessages.SendEmailBodyTemplate,
            createdOrder.OrderId, createdOrder.ProductsCount));
        }
    }

}
