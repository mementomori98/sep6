using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class InteractRequest : AuthorizedModel
    {
        public long DiscussionItemId { get; set; }
        public Interactions? Interaction { get; set; }
    }
}