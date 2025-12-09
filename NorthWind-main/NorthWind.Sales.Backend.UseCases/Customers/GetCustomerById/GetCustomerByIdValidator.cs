
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Customers.GetCustomerById
{
    internal class GetCustomerByIdValidator : IModelValidator<GetCustomerByIdDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint => ValidationConstraint.AlwaysValidate;

        public Task<bool> Validate(GetCustomerByIdDto model)
        {
            if (string.IsNullOrWhiteSpace(model.CustomerId))
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CustomerId),
                    "El Id del cliente no puede estar vacío"));
            }

            return Task.FromResult(!ErrorsField.Any());
        }
    }
}
