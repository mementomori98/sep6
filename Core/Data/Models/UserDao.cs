namespace Core.Data.Models
{
    public class UserDao
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}