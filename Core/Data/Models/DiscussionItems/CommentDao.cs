using Core.Domain.DiscussionItems.Models;
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

        public Comment MapToComment(long numberOfLikes, long numberOfDislikes, UserDiscussionItemInteractionType? interaction)
        {
            Comment copy = new Comment() { 
                DiscussionItemId = DiscussionItemId
            };

            return (Comment)MapToDiscussionItem(copy, numberOfLikes, numberOfDislikes, interaction);
        }

        public static CommentDao MapCommentToDao(Comment comment)
        {
            return (CommentDao) MapDiscussionItemToDao(
                comment, 
                new CommentDao()
                    {
                        DiscussionItemId = comment.DiscussionItemId
                    }
            );
        }
    }
}
