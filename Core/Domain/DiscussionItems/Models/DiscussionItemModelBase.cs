using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.DiscussionItems.Models
{
    public abstract class DiscussionItemModelBase
    {
        public long Id { get; set; }
        public string AuthorUsername { get; set; }
        public long AuthorId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        public long? DiscussableId { get; set; }
        public long NumberOfLikes { get; set; }
        public long NumberOfDislikes { get; set; }
        public UserDiscussionItemInteractionTypes? UserInteractionType { get; set; }
    }
}
