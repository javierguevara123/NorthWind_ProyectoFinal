using NorthWind.Sales.Entities.Dtos.Products.GetProductById;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Products.GetProductById
{
    /// <summary>
    /// Validador para GetProductByIdDto.
    /// </summary>
    internal class GetProductByIdValidator : IModelValidator<GetProductByIdDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint => ValidationConstraint.AlwaysValidate;

        public Task<bool> Validate(GetProductByIdDto model)
        {
            // Validar que el ID sea mayor que 0
            if (model.ProductId <= 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.ProductId),
                    "El Id del producto debe ser mayor que cero"));
            }

            return Task.FromResult(!ErrorsField.Any());
        }
    }
}
