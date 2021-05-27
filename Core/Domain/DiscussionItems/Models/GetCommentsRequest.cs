using System;

namespace Core.Domain.DiscussionItems.Models
{
    public class GetCommentsRequest
    {
        public long? DiscussableId { get; set; }
        public long? DiscussionItemId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Limit { get; set; }
    }
}