using NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer;
using NorthWind.Sales.Validators.Entities.Resources;
using NorthWind.Validation.Entities.Abstractions;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Validators.Entities.Customers.UpdateCustomer
{
    internal class UpdateCustomerDtoValidator : AbstractModelValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator(IValidationService<UpdateCustomerDto> validationService)
            : base(validationService)
        {
            AddRuleFor(c => c.CustomerId)
                .NotEmpty(UpdateCustomerMessages.CustomerIdRequired)
                .MaximumLength(20, UpdateCustomerMessages.CustomerIdMaximumLength);

            AddRuleFor(c => c.Name)
                .NotEmpty(UpdateCustomerMessages.NameRequired)
                .MaximumLength(40, UpdateCustomerMessages.NameMaximumLength);

            AddRuleFor(c => c.CurrentBalance)
                .GreaterThanOrEqualTo(0m, UpdateCustomerMessages.CurrentBalanceCannotBeNegative);
        }
    }
}

