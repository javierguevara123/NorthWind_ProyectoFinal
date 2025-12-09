namespace NorthWind.Sales.Backend.BusinessObjects.Entities
{
    /// <summary>
    /// Entidad simple de dominio para Customer.
    /// Sin reglas de negocio complejas.
    /// </summary>
    public class Customer
    {
        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }



    }
}