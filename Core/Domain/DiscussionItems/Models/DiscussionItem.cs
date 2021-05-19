using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.DiscussionItems.Models
{
    public abstract class DiscussionItem
    {
        //comment's/review's/fun fact's id
        public long Id { get; set; }
        public string Text { get; set; }
        public string AuthorUsername { get; set; }
        //movie/toplist/actor id
        public long DiscussableId { get; set; }
        public string NumberOfLikes { get; set; }
        public string NumberOfDislikes { get; set; }
        public bool LikedByUser { get; set; }
        public bool DislikedByUser { get; set; }
    }
}
