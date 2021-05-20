using Core.Data.Models.DiscussionItems;
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
        Task<long> AddComment(Comment comment);
        Task<long> AddFunFact(FunFact funFact);
        Task<long> AddReview(Review review);
        Task<long> AddSubcomment(Comment comment);
        Task<UserDiscussionItemInteraction> LikeDiscussionItem(long discussionItemId, long userId);
        Task<UserDiscussionItemInteraction> DislikeDiscussionItem(long discussionItemId, long userId);
    }
}
