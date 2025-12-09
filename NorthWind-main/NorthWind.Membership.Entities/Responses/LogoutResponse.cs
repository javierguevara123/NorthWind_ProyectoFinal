namespace NorthWind.Membership.Entities.Responses
{
    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
