using System;

namespace Core.Domain.Authentication.Models
{
    public class AuthToken
    {
        public string Value { get; set; }
        public DateTime Expiry { get; set; }

        public bool HasExpired()
        {
            return Expiry < DateTime.UtcNow;
        }
    }
}