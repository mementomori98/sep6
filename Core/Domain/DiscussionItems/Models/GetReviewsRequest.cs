using System;
using Core.Domain.Utils;

namespace Core.Domain.DiscussionItems.Models
{
    public class GetReviewsRequest : AuthorizedModel
    {
        public long? DiscussableId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Limit { get; set; }
    }
}