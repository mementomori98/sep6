﻿using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models.DiscussionItems
{
    public class UserDiscussionItemInteraction
    {
        public long DiscussionItemId { get; set; }
        public long UserId { get; set; }
        public UserDiscussionItemInteractionType InteractionType { get; set; }
    }
}
