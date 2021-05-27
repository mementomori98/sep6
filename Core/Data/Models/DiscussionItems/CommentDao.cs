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

        public CommentModel MapToComment(long numberOfLikes, long numberOfDislikes, Interactions? interaction)
        {
            CommentModel copy = new CommentModel() { 
                DiscussionItemId = DiscussionItemId
            };

            return (CommentModel)MapToDiscussionItem(copy, numberOfLikes, numberOfDislikes, interaction);
        }

        public static CommentDao MapCommentToDao(CommentModel comment)
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
