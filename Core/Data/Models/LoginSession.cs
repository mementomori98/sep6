using System;

namespace Core.Data.Models
{
    public class LoginSession
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        
        public long UserId { get; set; }
        public User User { get; set; }
    }
}