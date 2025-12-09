using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Orders.GetOrders
{
    internal class GetOrdersValidator : IModelValidator<GetOrdersQueryDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;
        public ValidationConstraint Constraint => ValidationConstraint.AlwaysValidate;

        public Task<bool> Validate(GetOrdersQueryDto model)
        {
            model.Validate();

            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                if (model.FromDate.Value > model.ToDate.Value)
                {
                    ErrorsField.Add(new ValidationError(
                        nameof(model.FromDate),
                        "La fecha inicial no puede ser mayor que la fecha final"));
                }
            }

            if (model.MinAmount.HasValue && model.MaxAmount.HasValue)
            {
                if (model.MinAmount.Value > model.MaxAmount.Value)
                {
                    ErrorsField.Add(new ValidationError(
                        nameof(model.MinAmount),
                        "El monto mínimo no puede ser mayor que el monto máximo"));
                }
            }

            return Task.FromResult(!ErrorsField.Any());
        }
    }
}
