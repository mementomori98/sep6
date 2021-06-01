using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class GetUserReviewRequest : AuthorizedModel
    {
        public long DiscussableId { get; set; }
    }
}