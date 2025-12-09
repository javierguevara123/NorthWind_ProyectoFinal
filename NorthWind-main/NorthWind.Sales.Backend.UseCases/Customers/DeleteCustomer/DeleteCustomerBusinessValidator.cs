using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;

//using NorthWind.Sales.Entities.Dtos.DeleteCustomer;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Customers.DeleteCustomer
{
    internal class DeleteCustomerBusinessValidator(IQueriesRepository repository)
        : IModelValidator<DeleteCustomerDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint =>
            ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(DeleteCustomerDto model)
        {
            // 1. Verificar existencia del cliente
            var exists = await repository.CustomerExists(model.CustomerId);
            if (!exists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CustomerId),
                    string.Format(
                        DeleteCustomerMessages.CustomerNotFoundTemplate,
                        model.CustomerId)));

                return false;
            }

            // 2. Verificar que no tenga saldo pendiente
            var balance = await repository.GetCustomerCurrentBalance(model.CustomerId);
            if (balance.HasValue && balance.Value > 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CustomerId),
                    string.Format(
                        DeleteCustomerMessages.CustomerHasBalanceTemplate,
                        model.CustomerId,
                        balance.Value)));
            }

            // 3. Verificar que no tenga órdenes pendientes
            var hasOrders = await repository.CustomerHasPendingOrders(model.CustomerId);
            if (hasOrders)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CustomerId),
                    string.Format(
                        DeleteCustomerMessages.CustomerPendingOrdersTemplate,
                        model.CustomerId)));
            }

            return !ErrorsField.Any();
        }
    }
}
