using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;
using NorthWind.Sales.Validators.Entities.Resources;
using NorthWind.Validation.Entities.Abstractions;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Validators.Entities.Customers.CreateCustomer
{
    internal class CreateCustomerDtoValidator : AbstractModelValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator(IValidationService<CreateCustomerDto> validationService)
            : base(validationService)
        {
            AddRuleFor(c => c.Id)
                .NotEmpty(CreateCustomerMessages.IdRequired)
                .MaximumLength(20, CreateCustomerMessages.IdMaximumLength);

            AddRuleFor(c => c.Name)
                .NotEmpty(CreateCustomerMessages.NameRequired)
                .MaximumLength(40, CreateCustomerMessages.NameMaximumLength);

            AddRuleFor(c => c.CurrentBalance)
                .GreaterThanOrEqualTo(0m, CreateCustomerMessages.CurrentBalanceCannotBeNegative);
        }
    }
}
