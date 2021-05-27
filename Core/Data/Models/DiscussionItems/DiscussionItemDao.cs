using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data.Models.DiscussionItems;

namespace Core.Data.Models
{
    public class DiscussionItemDao
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
        public long AuthorId { get; set; }
        public long? DiscussableId { get; set; }
        public UserDao Author { get; set; }
        
        public ICollection<UserDiscussionItemInteractionDao> Interactions { get; set; }
        public ICollection<CommentDao> Comments { get; set; }
        
        public DiscussionItemModelBase MapToDiscussionItem(DiscussionItemModelBase copy, long numberOfLikes, long numberOfDislikes, UserDiscussionItemInteractionTypes? interaction)
        {
            copy.Id = Id;
            copy.Text = Text;
            copy.AuthorUsername = Author.Username;
            copy.AuthorId = Author.Id;
            copy.DiscussableId = DiscussableId;
            copy.NumberOfLikes = numberOfLikes;
            copy.NumberOfDislikes = numberOfDislikes;
            copy.UserInteractionType = interaction;

            return copy;
        }

        public static DiscussionItemDao MapDiscussionItemToDao(DiscussionItemModelBase item, DiscussionItemDao dao)
        {
            dao.Id = item.Id;
            dao.Text = item.Text;
            dao.DiscussableId = item.DiscussableId;
            dao.AuthorId = item.AuthorId;

            return dao;
        }
    }
}
