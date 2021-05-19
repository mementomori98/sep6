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
        public long? FunFactId { get; set; }
        public long? ReviewId { get; set; }
        public long? CommentId { get; set; }

        public Comment MapToComment(string username, int numberOfLikes, int numberOfDislikes, bool likedByUser, bool dislikedByUser)
        {
            long discussableId;
            if (this.MovieId != null)
                discussableId = (long)this.MovieId;
            return new Comment()
            {
                Id = this.Id,
                Text = this.Text,
                AuthorUsername = username,
                //discussableId

            }
        }
    }
}
