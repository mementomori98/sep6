using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.DiscussionItems
{
    public interface IDiscussionItemService
    {
        Task<IEnumerable<Comment>> GetCommentsOnDiscussable(long discussableId);
        Task<IEnumerable<Review>> GetReviewsOnDiscussable(long discussableId);
        Task<IEnumerable<FunFact>> GetFunFactsOnDiscussable(long discussableId);
        Task<IEnumerable<Comment>> GetCommentsOnDiscussionItem(long discussionItemId);
        Task AddComment(Comment comment);
        Task AddFunFact(FunFact funFact);
        Task AddReview(Review review);
        Task AddSubcomment(Comment comment);
    }
}
