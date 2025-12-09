namespace NorthWind.Sales.Backend.BusinessObjects.Entities
{
    /// <summary>
    /// Entidad simple de dominio para Product.
    /// Sin reglas de negocio complejas.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public short UnitsInStock { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
