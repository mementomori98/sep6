using System;

namespace Core.Services.Authentication.Models
{
    public class AuthToken
    {
        public string Value { get; set; }
        public DateTime Expiry { get; set; }
    }
}