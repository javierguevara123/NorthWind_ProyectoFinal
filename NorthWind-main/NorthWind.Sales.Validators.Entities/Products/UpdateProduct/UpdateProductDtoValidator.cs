using NorthWind.Sales.Entities.Dtos.Products.UpdateProduct;
using NorthWind.Sales.Validators.Entities.Resources;
using NorthWind.Validation.Entities.Abstractions;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Sales.Validators.Entities.Products.UpdateProduct
{
    internal class UpdateProductDtoValidator : AbstractModelValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator(IValidationService<UpdateProductDto> validationService) : base(validationService)
        {
            AddRuleFor(p => p.ProductId)
                .GreaterThan(0, UpdateProductMessages.IdMustBeGreaterThanZero);

            AddRuleFor(p => p.Name)
                .NotEmpty(UpdateProductMessages.NameRequired)
                .MaximumLength(40, UpdateProductMessages.NameMaximumLength);

            AddRuleFor(p => p.UnitsInStock)
                .GreaterThanOrEqualTo((short)0, UpdateProductMessages.UnitsInStockCannotBeNegative)
                .LessThanOrEqualTo((short)32767, UpdateProductMessages.UnitsInStockExceedsLimit);

            AddRuleFor(p => p.UnitPrice)
                .GreaterThan(0m, UpdateProductMessages.UnitPriceMustBeGreaterThanZero)
                .LessThanOrEqualTo(999999.99m, UpdateProductMessages.UnitPriceExceedsLimit);
        }
    }
}
