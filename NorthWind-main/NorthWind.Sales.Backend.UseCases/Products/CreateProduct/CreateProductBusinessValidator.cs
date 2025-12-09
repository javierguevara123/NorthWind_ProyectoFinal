using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Backend.UseCases.Resources;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Products.CreateProduct
{
    /// <summary>
    /// Validador de reglas de negocio para CreateProduct.
    /// Verifica que el nombre del producto no esté duplicado.
    /// </summary>
    internal class CreateProductBusinessValidator(IQueriesRepository repository)
        : IModelValidator<CreateProductDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;

        public ValidationConstraint Constraint =>
            ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(CreateProductDto model)
        {
            // Verificar que el nombre del producto no esté duplicado
            var nameExists = await repository.ProductNameExists(model.Name);
            if (nameExists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Name),
                    string.Format(
                        CreateProductMessages.ProductNameAlreadyExistsErrorTemplate,
                        model.Name)));
            }

            return !ErrorsField.Any();
        }
    }
}
