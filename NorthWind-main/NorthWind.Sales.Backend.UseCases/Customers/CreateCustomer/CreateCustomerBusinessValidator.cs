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
            // 1️⃣ Verificar si ya existe un cliente con ese NOMBRE
            if (await repository.CustomerNameExists(model.Name))
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Name),
                    string.Format(CreateCustomerMessages.CustomerAlreadyExistsTemplate, model.Name)));
            }

            // 2️⃣ NUEVO: Verificar si ya existe un cliente con ese EMAIL
            if (await repository.CustomerEmailExists(model.Email))
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Email),
                    $"El correo electrónico '{model.Email}' ya está registrado."));
            }

            // 3️⃣ NUEVO: Verificar si ya existe un cliente con esa CÉDULA
            if (await repository.CustomerCedulaExists(model.Cedula))
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Cedula),
                    $"El número de cédula '{model.Cedula}' ya se encuentra registrado."));
            }

            // 4️⃣ Validación de longitud del ID
            // Nota: Corregí las llaves {} aquí, estaban faltando en tu código original
            if (model.Id.Length > 10) // Usualmente es 5 en Northwind, ajusta a 10 si cambiaste la BD
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.Id),
                    "El código de cliente debe tener máximo 10 caracteres."));
            }

            // 5️⃣ Regla opcional: no permitir saldos negativos
            if (model.CurrentBalance < 0)
            {
                ErrorsField.Add(new ValidationError(
                    nameof(model.CurrentBalance),
                    CreateCustomerMessages.NegativeBalanceError));
            }

            // Retorna TRUE si la lista de errores está vacía
            return !ErrorsField.Any();
        }
    }
}