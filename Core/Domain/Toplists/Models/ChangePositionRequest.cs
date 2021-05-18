using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class ChangePositionRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
        public long MovieId { get; set; }
        public int Position { get; set; }
    }
}