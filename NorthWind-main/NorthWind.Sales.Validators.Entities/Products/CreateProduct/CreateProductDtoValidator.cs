using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;
using NorthWind.Sales.Validators.Entities.Resources;
using NorthWind.Validation.Entities.Abstractions;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Validators.Entities.Products.CreateProduct
{
    internal class CreateProductDtoValidator : AbstractModelValidator<CreateProductDto>
    {
        public CreateProductDtoValidator(IValidationService<CreateProductDto> validationService)
            : base(validationService)
        {
            AddRuleFor(p => p.Name)
                .NotEmpty(CreateProductMessages.NameRequired)
                .MaximumLength(40, CreateProductMessages.NameMaximumLength);

            AddRuleFor(p => p.UnitsInStock)
                .GreaterThanOrEqualTo((short)0, CreateProductMessages.UnitsInStockCannotBeNegative)
                .LessThanOrEqualTo((short)32767, CreateProductMessages.UnitsInStockExceedsLimit);

            AddRuleFor(p => p.UnitPrice)
                .GreaterThan(0m, CreateProductMessages.UnitPriceMustBeGreaterThanZero)
                .LessThanOrEqualTo(999999.99m, CreateProductMessages.UnitPriceExceedsLimit);
        }
    }
}
