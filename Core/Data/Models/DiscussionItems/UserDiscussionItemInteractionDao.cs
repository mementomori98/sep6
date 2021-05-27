using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models.DiscussionItems
{
    public class UserDiscussionItemInteractionDao
    {
        public long DiscussionItemId { get; set; }
        public long UserId { get; set; }
        public UserDiscussionItemInteractionTypes InteractionType { get; set; }
    }
}
