using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class AddCommentRequest : AuthorizedModel
    {
        public long? DiscussableId { get; set; }
        public long? DiscussionItemId { get; set; }
        public string Text { get; set; }
    }
}