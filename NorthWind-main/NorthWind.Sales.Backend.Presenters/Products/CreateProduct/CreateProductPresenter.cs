using NorthWind.Sales.Backend.BusinessObjects.Entities;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct;

namespace NorthWind.Sales.Backend.Presenters.Products.CreateProduct
{
    internal class CreateProductPresenter : ICreateProductOutputPort
    {
        public int ProductId { get; private set; }
        public Task Handle(Product addedProduct)
        {
            ProductId = addedProduct.Id;
            return Task.CompletedTask;
        }
    }
}
