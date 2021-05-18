namespace Core.Domain.Authentication.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Persist { get; set; }
    }
}