using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Orders.GetOrderById
{
    internal class GetOrderByIdValidator : IModelValidator<GetOrderByIdDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;
        public ValidationConstraint Constraint => ValidationConstraint.AlwaysValidate;

        public Task<bool> Validate(GetOrderByIdDto model)
        {
            if (model.OrderId <= 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.OrderId),
                    "El Id de la orden debe ser mayor que cero"));
            }

            return Task.FromResult(!ErrorsField.Any());
        }
    }
}
