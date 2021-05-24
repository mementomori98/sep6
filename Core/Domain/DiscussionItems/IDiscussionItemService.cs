﻿using Core.Data.Models.DiscussionItems;
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
        Task<PageResult<Comment>> GetCommentsOnDiscussable(long discussableId, int page, long userId);
        Task<PageResult<Review>> GetReviewsOnDiscussable(long discussableId, int page, long userId);
        Task<PageResult<FunFact>> GetFunFactsOnDiscussable(long discussableId, int page, long userId);
        Task<PageResult<Comment>> GetSubcommentsOnDiscussionItem(long discussionItemId, int page, long userId);
        Task<long> AddComment(Comment comment);
        Task<long> AddFunFact(FunFact funFact);
        Task<long> AddReview(Review review);
        Task<long> AddSubcomment(Comment comment);
        Task<UserDiscussionItemInteraction> LikeDiscussionItem(long discussionItemId, long userId);
        Task<UserDiscussionItemInteraction> DislikeDiscussionItem(long discussionItemId, long userId);
    }
}
