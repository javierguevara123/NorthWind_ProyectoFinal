using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Customers.CreateCustomer
{
    internal class CreateCustomerBusinessValidator(IQueriesRepository repository)
        : IModelValidator<CreateCustomerDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint =>
            ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(CreateCustomerDto model)
        {
            // 1️⃣ verificar si ya existe un cliente con ese nombre
            bool nameExists = await repository.CustomerNameExists(model.Name);

            if (nameExists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Name),
                    string.Format(
                        CreateCustomerMessages.CustomerAlreadyExistsTemplate,
                        model.Name)));
            }

            if (model.Id.Length != 5)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Id),
                    "El código de cliente debe tener exactamente 5 caracteres."));
                return false;
            }

            // 2️⃣ regla opcional: no permitir saldos negativos
            if (model.CurrentBalance < 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CurrentBalance),
                    CreateCustomerMessages.NegativeBalanceError));
            }

            return !ErrorsField.Any();
        }
    }
}
