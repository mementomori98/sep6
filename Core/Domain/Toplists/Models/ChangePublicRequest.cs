using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class ChangePublicRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
        public bool Public { get; set; }
    }
}