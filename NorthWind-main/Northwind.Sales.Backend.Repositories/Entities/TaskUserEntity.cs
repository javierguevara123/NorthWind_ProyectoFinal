namespace NorthWind.Sales.Backend.Repositories.Entities
{
    public class TaskUser
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "Personal";    // "Personal" o "Negocio"
        public bool IsCompleted { get; set; } = false;

        // Campo para vincular la tarea con el usuario del token
        public string UserId { get; set; } = string.Empty;
    }
}
