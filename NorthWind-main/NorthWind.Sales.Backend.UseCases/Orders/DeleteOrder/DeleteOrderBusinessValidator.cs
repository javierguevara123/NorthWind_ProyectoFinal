using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories;
using NorthWind.Sales.Entities.Dtos.Orders.DeleteOrder;
using NorthWind.Validation.Entities.Enums;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Sales.Backend.UseCases.Orders.DeleteOrder
{
    internal class DeleteOrderBusinessValidator(IQueriesRepository repository)
        : IModelValidator<DeleteOrderDto>
    {
        private readonly List<ValidationError> ErrorsField = [];

        public IEnumerable<ValidationError> Errors => ErrorsField;
        public ValidationConstraint Constraint => ValidationConstraint.ValidateIfThereAreNoPreviousErrors;

        public async Task<bool> Validate(DeleteOrderDto model)
        {
            // Verificar existencia
            var exists = await repository.OrderExists(model.OrderId);
            if (!exists)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.OrderId),
                    $"La orden con Id {model.OrderId} no existe"));
            }

            return !ErrorsField.Any();
        }
    }
}
