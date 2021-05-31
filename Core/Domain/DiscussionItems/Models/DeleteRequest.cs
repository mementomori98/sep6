using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class DeleteRequest : AuthorizedModel
    {
        public long DiscussionItemId { get; set; }
    }
}