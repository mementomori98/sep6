using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class RenameToplistRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
        public string Name { get; set; }
    }
}