using Core.Domain.Authentication.Models;
using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class CreateToplistRequest : AuthorizedModel
    {
        public string Name { get; set; }
        public bool Public { get; set; }
    }
}