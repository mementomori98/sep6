using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class AddReviewRequest : AuthorizedModel
    {
        public long DiscussableId { get; set; }
        public long Text { get; set; }
        public int Stars { get; set; }
    }
}