using Core.Domain.DiscussionItems.Models;
using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.DiscussionItems
{
    class DiscussionItemService
    {
        public async Task<IEnumerable<Models.Comment>> GetCommentsOnDiscussable(long discussableId)
        {
            await using var context = new MovieContext();
            var commentsDao = await context.Set<Data.Models.CommentDao>()
                .Where(c => c.MovieId == discussableId || c.ActorId == discussableId || c.ToplistId == discussableId)
                .Take(10)
                .ToListAsync();
            List<Comment> comments = new List<Comment>();
            foreach(var commentDao in commentsDao)
            {
                var user = await context.Set<User>().SingleOrDefaultAsync(u => u.Id == commentDao.AuthorId);
                context.Set<>
                commentDao.MapToComment(user.Username);
            }
            return comments;
        }

        public Comment GetCommentFromDao()
        {
            return new Comment()
            {

            };
        }
    }
}
