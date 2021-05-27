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
using Core.Domain.Authentication;
using Org.BouncyCastle.Asn1.X509;

namespace Core.Domain.DiscussionItems
{
    public class DiscussionItemService : IDiscussionItemService
    {
        private const int DefaultPageSize = 10;
        private readonly IAuthenticationService _authenticationService;

        public DiscussionItemService(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<PageResult<CommentModel>> GetComments(GetCommentsRequest request)
        {
            await using var context = new MovieContext();
            var query = GetQuery<CommentDao>(context, request.DiscussableId, request.FromDate, request.ToDate)
                .Where(c => c.DiscussionItemId == request.DiscussionItemId);

            var limit = request.Limit ?? DefaultPageSize;
            var count = await query.CountAsync();

            var comments = await query
                .Take(limit)
                .ToListAsync();

            var user = await _authenticationService.GetCurrentUser(request.Token);

            return new PageResult<CommentModel>
            {
                Items = comments.OrderBy(c => c.Created).Select(c => Map(c, user?.Id)),
                HasMore = count > limit
            };
        }

        public async Task<PageResult<ReviewModel>> GetReviews(GetReviewsRequest request)
        {
            await using var context = new MovieContext();
            var query = GetQuery<ReviewDao>(context, request.DiscussableId, request.FromDate, request.ToDate);

            var limit = request.Limit ?? DefaultPageSize;
            var count = await query.CountAsync();

            var reviews = await query
                .Take(limit)
                .ToListAsync();

            var user = await _authenticationService.GetCurrentUser(request.Token);

            return new PageResult<ReviewModel>
            {
                Items = reviews.OrderBy(r => r.Created).Select(r => Map(r, user?.Id)),
                HasMore = count > limit
            };
        }

        public async Task<PageResult<FunFactModel>> GetFunFacts(GetFunFactsRequest request)
        {
            await using var context = new MovieContext();
            var query = GetQuery<FunFactDao>(context, request.DiscussableId, request.FromDate, request.ToDate);

            var limit = request.Limit ?? DefaultPageSize;
            var count = await query.CountAsync();

            var funFacts = await query
                .Take(limit)
                .ToListAsync();

            var user = await _authenticationService.GetCurrentUser(request.Token);

            return new PageResult<FunFactModel>
            {
                Items = funFacts.OrderBy(f => f.Created).Select(f => Map(f, user?.Id)),
                HasMore = count > limit
            };
        }

        public async Task<CommentModel> AddComment(AddCommentRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            if (request.DiscussableId != null && request.DiscussionItemId != null)
                throw new ArgumentException("Can't have both ids");
            if (request.DiscussableId == null && request.DiscussionItemId == null)
                throw new ArgumentException("Must have one id");

            await using var context = new MovieContext();
            var now = DateTime.Now;
            var entry = await context.AddAsync(new CommentDao
            {
                Created = now,
                Text = request.Text,
                DiscussableId = request.DiscussableId,
                DiscussionItemId = request.DiscussionItemId,
                AuthorId = user.Id
            });

            await context.SaveChangesAsync();

            var query = GetQuery<CommentDao>(context);

            return Map(await query.SingleAsync(c => c.Id == entry.Entity.Id), null);
        }

        public async Task<ReviewModel> AddReview(AddReviewRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var now = DateTime.Now;
            var entry = await context.AddAsync(new ReviewDao
            {
                Created = now,
                Text = request.Text,
                DiscussableId = request.DiscussableId,
                NumberOfStars = request.Stars,
                AuthorId = user.Id
            });

            await context.SaveChangesAsync();

            var query = GetQuery<ReviewDao>(context);

            return Map(await query.SingleAsync(r => r.Id == entry.Entity.Id), null);
        }

        public async Task<FunFactModel> AddFunFact(AddFunFactRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var now = DateTime.Now;
            var entry = await context.AddAsync(new FunFactDao
            {
                Created = now,
                Text = request.Text,
                AuthorId = user.Id,
                DiscussableId = request.DiscussableId
            });

            await context.SaveChangesAsync();

            var query = GetQuery<FunFactDao>(context);

            return Map(await query.SingleAsync(f => f.Id == entry.Entity.Id), null);
        }

        public async Task Interact(InteractRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var interaction = await context.Set<InteractionDao>()
                .SingleOrDefaultAsync(i => i.UserId == user.Id &&
                                  i.DiscussionItemId == request.DiscussionItemId);
            if (request.Interaction != null && interaction == null)
                await context.AddAsync(new InteractionDao
                {
                    UserId = user.Id,
                    DiscussionItemId = request.DiscussionItemId,
                    Type = request.Interaction.Value
                });
            else if (request.Interaction != null && interaction != null)
            {
                interaction.Type = request.Interaction.Value;
                context.Update(interaction);
            }
            else if (request.Interaction == null && interaction != null)
            {
                context.Remove(interaction);
            }

            await context.SaveChangesAsync();
        }

        public async Task Delete(DeleteRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null)
                throw new Exception("Unauthorized");

            await using var context = new MovieContext();
            var item = await context.Set<DiscussionItemDao>()
                .SingleAsync(di => di.Id == request.DiscussionItemId);
            if (item.AuthorId != user.Id)
                throw new Exception("User does not own Discussion Item");

            context.Remove(item);
            await context.SaveChangesAsync();
        }

        private static IQueryable<TDao> GetQuery<TDao>(MovieContext context, long? discussableId = null, DateTime? fromDate = null, DateTime? toDate = null) where TDao : DiscussionItemDao
        {
            var query = context.Set<TDao>()
                .Include(di => di.Author)
                .Include(di => di.Interactions)
                .Include(di => di.Comments)
                .AsQueryable();
            
            if (discussableId != null)
                query = query.Where(di => di.DiscussableId == discussableId);
            if (toDate != null)
                query = query.Where(di => di.Created < toDate.Value)
                    .OrderByDescending(di => di.Created);
            if (fromDate != null)
                query = query.Where(di => di.Created > fromDate.Value)
                    .OrderBy(di => di.Created);

            return query;
        }
        
        private static CommentModel Map(CommentDao comment, long? userId)
        {
            var model = MapBase<CommentModel, CommentDao>(comment, userId);
            model.DiscussionItemId = comment.DiscussableId;
            return model;
        }

        private static ReviewModel Map(ReviewDao review, long? userId)
        {
            var model = MapBase<ReviewModel, ReviewDao>(review, userId);
            model.NumberOfStars = review.NumberOfStars;
            return model;
        }

        private static FunFactModel Map(FunFactDao funFact, long? userId)
        {
            return MapBase<FunFactModel, FunFactDao>(funFact, userId);
        }

        private static TModel MapBase<TModel, TDao>(TDao dao, long? userId = null)
            where TModel : DiscussionItemModelBase, new()
            where TDao : DiscussionItemDao
        {
            return new TModel
            {
                Id = dao.Id,
                Created = dao.Created,
                Text = dao.Text,
                AuthorId = dao.AuthorId,
                AuthorUsername = dao.Author.Username,
                HasComments = dao.Comments.Any(),
                DiscussableId = dao.DiscussableId,
                NumberOfLikes = dao.Interactions.Count(i => i.Type == Interactions.Like),
                NumberOfDislikes = dao.Interactions.Count(i => i.Type == Interactions.Dislike),
                UserInteractionType = dao.Interactions.SingleOrDefault(i => i.UserId == userId)?.Type
            };
        }
    }
}