﻿using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class CommentDao : DiscussionItemDao
    {
        public long? DiscussionItemId { get; set; }
    }
}
