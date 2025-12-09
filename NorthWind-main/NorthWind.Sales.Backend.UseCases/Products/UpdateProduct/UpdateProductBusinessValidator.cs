using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Products.UpdateProduct
{
    /// <summary>
    /// Validador de reglas de negocio para UpdateProduct.
    /// Verifica que el producto exista y valida restricciones de negocio.
    /// </summary>
    internal class UpdateProductBusinessValidator(IQueriesRepository repository) : IModelValidator<UpdateProductDto>
    {
        private readonly List<ValidationError> ErrorsField = [];
        public IEnumerable<ValidationError> Errors => ErrorsField;
        public ValidationConstraint Constraint => ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(UpdateProductDto model)
        {
            // 1. Verificar que el producto existe
            var productExists = await repository.ProductExists(model.ProductId);

            if (!productExists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.ProductId),
                    string.Format(
                        UpdateProductMessages.ProductIdNotFoundErrorTemplate,
                        model.ProductId)));

                // Si el producto no existe, no continuar con otras validaciones
                return false;
            }

            // 2. Obtener el producto actual (opcional, si necesitas validar cambios)
            var currentProduct = await repository.GetProductById(model.ProductId);

            if (currentProduct != null)
            {
                // 3. Validar que no se reduzca el stock por debajo de las unidades comprometidas
                // (Opcional: solo si tienes lógica de unidades comprometidas en órdenes pendientes)
                if (model.UnitsInStock < currentProduct.UnitsInStock)
                {
                    // Verificar si hay órdenes pendientes que requieren ese stock
                    var committedUnits = await repository.GetCommittedUnits(model.ProductId);

                    if (model.UnitsInStock < committedUnits)
                    {
                        ErrorsField.Add(new ValidationError(
                            nameof(model.UnitsInStock),
                            string.Format(
                                UpdateProductMessages.UnitsInStockBelowCommittedErrorTemplate,
                                committedUnits,
                                model.UnitsInStock,
                                model.ProductId)));
                    }
                }

                // 4. Validar que el nombre no esté duplicado (si cambió el nombre)
                if (!string.Equals(currentProduct.Name, model.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameExists = await repository.ProductNameExists(model.Name, model.ProductId);

                    if (nameExists)
                    {
                        ErrorsField.Add(new ValidationError(
                            nameof(model.Name),
                            string.Format(
                                UpdateProductMessages.ProductNameAlreadyExistsErrorTemplate,
                                model.Name)));
                    }
                }
            }

            return !ErrorsField.Any();
        }
    }
}
