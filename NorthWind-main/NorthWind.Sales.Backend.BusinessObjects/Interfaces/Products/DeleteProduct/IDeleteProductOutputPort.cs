namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.DeleteProduct
{
    public interface IDeleteProductOutputPort
    {
        int ProductId { get; }
        Task Handle(int productId);
    }
}
