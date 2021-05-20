using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class DiscussionItemDao
    {
        //comment's/review's/fun fact's id
        public long Id { get; set; }
        public string Text { get; set; }
        public long AuthorId { get; set; }
        //movie/toplist/actor id
        public long? DiscussableId { get; set; }
        public UserDao Author { get; set; }


        public DiscussionItem MapToDiscussionItem(DiscussionItem copy, long numberOfLikes, long numberOfDislikes, UserDiscussionItemInteractionType? interaction)
        {
            copy.Id = Id;
            copy.Text = Text;
            copy.AuthorUsername = Author.Username;
            copy.AuthorId = Author.Id;
            copy.DiscussableId = DiscussableId;
            copy.NumberOfLikes = numberOfLikes;
            copy.NumberOfDislikes = numberOfDislikes;
            copy.userInteractionType = interaction;

            return copy;
        }

        public static DiscussionItemDao MapDiscussionItemToDao(DiscussionItem item, DiscussionItemDao dao)
        {
            dao.Id = item.Id;
            dao.Text = item.Text;
            dao.DiscussableId = item.DiscussableId;
            dao.AuthorId = item.AuthorId;

            return dao;
        }
    }
}
