using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class AddFunFactRequest : AuthorizedModel
    {
        public long DiscussableId { get; set; }
        public long Text { get; set; }
    }
}