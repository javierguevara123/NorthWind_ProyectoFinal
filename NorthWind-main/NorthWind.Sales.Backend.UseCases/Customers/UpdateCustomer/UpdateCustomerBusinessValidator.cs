using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Customers.UpdateCustomer
{
    internal class UpdateCustomerBusinessValidator(IQueriesRepository repository)
        : IModelValidator<UpdateCustomerDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint =>
            ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(UpdateCustomerDto model)
        {
            // 1. Verificar que el cliente existe
            var exists = await repository.CustomerExists(model.CustomerId);

            if (!exists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CustomerId),
                    string.Format(
                        UpdateCustomerMessages.CustomerNotFoundTemplate,
                        model.CustomerId)));

                return false;
            }

            // 2. Obtener los datos actuales del cliente
            var current = await repository.GetCustomerById(model.CustomerId);

            if (current != null)
            {
                // 3. Validar que el nombre no exista en otro cliente
                if (!string.Equals(current.Name, model.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameExists = await repository.CustomerNameExists(model.Name, model.CustomerId);
                    if (nameExists)
                    {
                        ErrorsField.Add(new ValidationError(
                            nameof(model.Name),
                            string.Format(
                                UpdateCustomerMessages.CustomerNameAlreadyExistsTemplate,
                                model.Name)));
                    }
                }

                // 4. Validar que el balance no sea negativo
                if (model.CurrentBalance < 0)
                {
                    ErrorsField.Add(new ValidationError(
                        nameof(model.CurrentBalance),
                        UpdateCustomerMessages.CustomerBalanceCannotBeNegative));
                }
            }

            return !ErrorsField.Any();
        }
    }
}
