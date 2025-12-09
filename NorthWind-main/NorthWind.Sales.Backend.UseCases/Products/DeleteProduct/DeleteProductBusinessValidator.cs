using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Products.DeleteProduct
{
    /// <summary>
    /// Validador de reglas de negocio para DeleteProduct.
    /// </summary>
    internal class DeleteProductBusinessValidator(IQueriesRepository repository)
        : IModelValidator<DeleteProductDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint =>
            ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(DeleteProductDto model)
        {
            // 1. Verificar que el producto existe
            var productExists = await repository.ProductExists(model.ProductId);
            if (!productExists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.ProductId),
                    string.Format(
                        DeleteProductMessages.ProductIdNotFoundErrorTemplate,
                        model.ProductId)));
                return false;
            }

            // 2. Verificar que no tenga unidades comprometidas en órdenes pendientes
            var committedUnits = await repository.GetCommittedUnits(model.ProductId);
            if (committedUnits > 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.ProductId),
                    string.Format(
                        DeleteProductMessages.ProductHasCommittedUnitsErrorTemplate,
                        model.ProductId,
                        committedUnits)));
            }

            return !ErrorsField.Any();
        }
    }
}
