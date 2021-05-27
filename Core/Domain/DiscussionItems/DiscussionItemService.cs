using Core.Domain.DiscussionItems.Models;
using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Core.Data.Models.DiscussionItems;

namespace Core.Domain.DiscussionItems
{
    public class DiscussionItemService : IDiscussionItemService
    {
        MovieContext Context = new MovieContext();
        private const int DefaultPageSize = 10;

        public async Task<long> AddComment(CommentModel comment)
        {
            CommentDao dao = CommentDao.MapCommentToDao(comment);
            var entry = await Context.AddAsync(dao);
            await Context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddFunFact(FunFactModel funFact)
        {
            FunFactDao dao = FunFactDao.MapFunFactToDao(funFact);
            var entry = await Context.AddAsync(dao);
            await Context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddReview(ReviewModel review)
        {
            ReviewDao dao = ReviewDao.MapReviewToDao(review);
            var entry = await Context.AddAsync(dao);
            await Context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<long> AddSubcomment(CommentModel comment)
        {
            return await AddComment(comment);
        }

        public async Task<PageResult<CommentModel>> GetCommentsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = Context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussableId == discussableId)
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * DefaultPageSize);
            
            var commentsDao = await query
                .Take(DefaultPageSize)
                .ToListAsync();

            return new PageResult<CommentModel>
            {
                Items = await GetComments(commentsDao, page, userId),
                HasMore = await query.CountAsync() > DefaultPageSize
            };
        }

        public async Task<PageResult<CommentModel>> GetSubcommentsOnDiscussionItem(long discussionItemId, int page, long userId)
        {
            var query = Context.Set<CommentDao>()
                .Include(c => c.Author)
                .Where(c => c.DiscussionItemId == discussionItemId)
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * DefaultPageSize);
            
            var commentsDao = await query
                .Take(DefaultPageSize)
                .ToListAsync();

            return new PageResult<CommentModel>
            {
                Items = await GetComments(commentsDao, page, userId),
                HasMore = await query.CountAsync() > DefaultPageSize
            };
        }

        private async Task<IEnumerable<CommentModel>> GetComments(List<CommentDao> commentsDao, int page, long userId)
        {
            var comments = commentsDao.Select(async c => await ToComment(c, userId)).ToList();

            await Task.WhenAll(comments);
            return comments.Select(c => c.Result);
        }

        private async Task<CommentModel> ToComment(CommentDao dao, long userId)
        {
            return dao.MapToComment(
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<PageResult<ReviewModel>> GetReviewsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = Context.Set<ReviewDao>()
                .Include(r => r.Author)
                .Where(r => r.DiscussableId == discussableId)
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * DefaultPageSize);
            var reviewsDao = await query
                .Take(DefaultPageSize)
                .ToListAsync();

            var reviews = reviewsDao.Select(async r => await ToReview(r, userId)).ToList();

            await Task.WhenAll(reviews);
            return new PageResult<ReviewModel>
            {
                Items = reviews.Select(c => c.Result),
                HasMore = await query.CountAsync() > DefaultPageSize
            };
        }

        private async Task<ReviewModel> ToReview(ReviewDao dao, long userId)
        {
            return dao.MapToReview(
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<PageResult<FunFactModel>> GetFunFactsOnDiscussable(long discussableId, int page, long userId)
        {
            var query = Context.Set<FunFactDao>()
                .Include(f => f.Author)
                .Where(f => f.DiscussableId == discussableId)
                .OrderByDescending(f => f.Id)
                .Skip((page - 1) * DefaultPageSize);
            
            var funFactsDao = await query
                .Take(DefaultPageSize)
                .ToListAsync();

            var funFacts = funFactsDao.Select(async f => await ToFunFact(f, userId)).ToList();

            await Task.WhenAll(funFacts);
            return new PageResult<FunFactModel>
            {
                Items = funFacts.Select(c => c.Result),
                HasMore = await query.CountAsync() > DefaultPageSize
            };
        }

        private async Task<FunFactModel> ToFunFact(FunFactDao dao, long userId)
        {
            return dao.MapToFunFact(
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Like, dao.Id),
                await GetNumberOfInteractions(UserDiscussionItemInteractionTypes.Dislike, dao.Id),
                await GetUserInteraction(userId, dao.Id)
            );
        }

        public async Task<UserDiscussionItemInteractionDao> LikeDiscussionItem(long discussionItemId, long userId)
        {
            return await SaveUserInteractionOnDiscussionItem(discussionItemId, userId, UserDiscussionItemInteractionTypes.Like);
        }

        public async Task<UserDiscussionItemInteractionDao> DislikeDiscussionItem(long discussionItemId, long userId)
        {
            return await SaveUserInteractionOnDiscussionItem(discussionItemId, userId, UserDiscussionItemInteractionTypes.Dislike);
        }

        private async Task<UserDiscussionItemInteractionDao> SaveUserInteractionOnDiscussionItem(long discussionItemId, long userId, UserDiscussionItemInteractionTypes types)
        {
            var previousUserInteraction = await Context.Set<UserDiscussionItemInteractionDao>().SingleOrDefaultAsync(i => i.DiscussionItemId == discussionItemId && i.UserId == userId);
            if(previousUserInteraction != null)
                Context.Set<UserDiscussionItemInteractionDao>().Remove(previousUserInteraction);
            
            var entry = await Context.AddAsync<UserDiscussionItemInteractionDao>(
                new UserDiscussionItemInteractionDao()
                {
                    InteractionType = types,
                    DiscussionItemId = discussionItemId,
                    UserId = userId
                });
            await Context.SaveChangesAsync();
            return entry.Entity;
        }

        private async Task<long> GetNumberOfInteractions(UserDiscussionItemInteractionTypes interactionTypes, long discussionItemId)
        {
            return await Context.Set<UserDiscussionItemInteractionDao>()
                .CountAsync(i => i.DiscussionItemId == discussionItemId && i.InteractionType == interactionTypes);
        }

        private async Task<UserDiscussionItemInteractionTypes?> GetUserInteraction(long userId, long discussionItemId)
        {
            var userInteraction = await Context.Set<UserDiscussionItemInteractionDao>()
                .SingleOrDefaultAsync(i => i.DiscussionItemId == discussionItemId && i.UserId == userId);
            return userInteraction == null ? null : userInteraction.InteractionType;
        }

        public async Task<PageResult<CommentModel>> GetComments(GetCommentsRequest request)
        {
            await using var context = new MovieContext();
            var query = context.Set<CommentDao>()
                .Where(c => c.DiscussableId == request.DiscussableId &&
                            c.DiscussionItemId == request.DiscussionItemId);
            if (request.FromDate != null)
                query = query.Where(c => c.Created > request.FromDate.Value);
            if (request.ToDate != null)
                query = query.Where(c => c.Created < request.ToDate.Value);

            var limit = request.Limit ?? DefaultPageSize;
            var count = await query.CountAsync();

            var comments = await query
                .Take(limit)
                .ToListAsync();
            
            return new PageResult<CommentModel>
            {
                Items = comments.Select(Map),
                HasMore = count > limit
            };
        }

        public async Task<PageResult<ReviewModel>> GetReviews(GetReviewsRequest request)
        {
            await using var context = new MovieContext();
            return null;
        }

        public async Task<PageResult<FunFactModel>> GetFunFacts(GetFunFactsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<CommentModel> AddComment(AddCommentRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ReviewModel> AddReview(AddReviewRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<FunFactModel> AddFunFact(AddFunFactRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task Interact(InteractRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(DeleteRequest request)
        {
            throw new NotImplementedException();
        }

        private static CommentModel Map(CommentDao comment)
        {
            return null;
        }
    }
}
