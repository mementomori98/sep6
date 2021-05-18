using Core.Domain.Utils;

namespace Core.Domain.Toplists.Models
{
    public class DeleteToplistRequest : AuthorizedModel
    {
        public long ToplistId { get; set; }
    }
}