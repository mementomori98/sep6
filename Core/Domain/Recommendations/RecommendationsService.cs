using Core.Data;
using Core.Data.Models;
using Core.Domain.Authentication;
using Core.Domain.DiscussionItems;
using Core.Domain.Movies;
using Core.Domain.Movies.Models;
using Core.Domain.Recommendations.Models;
using Core.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations
{
    public class RecommendationsService : IRecommendationService
    {
        private readonly IDiscussionItemService _discussionItemService;
        private readonly IMovieRecommendationService _movieService;
        private readonly IAuthenticationService _authenticationService;
        private string ApiKey = "d7c20c31c1ae99c2b49085a9c733817e";
        private static HttpClient client = new HttpClient();

        public RecommendationsService(IDiscussionItemService discussionItemService, IAuthenticationService authenticationService, IMovieRecommendationService movieService)
        {
            _discussionItemService = discussionItemService;
            _authenticationService = authenticationService;
            _movieService = movieService;
        }

        public async Task<List<MovieDao>> GetRecommendationsForUser(RecommendationRequest request)
        {
            var user = await _authenticationService.GetCurrentUser(request.Token);
            if (user == null) return await GetTopRated(GetRecommendationsGetModelForUnauthenticatedUser(request.RequestNumber));
            return await GetRecommendations(await GetRecommendationsGetModel(user.Id, request), user.Id);
        }

        private RecommendationsGetModel GetRecommendationsGetModelForUnauthenticatedUser(int requestNumber)
        {
            return new RecommendationsGetModel() { TopRatedRecommendationPageNumber = requestNumber};
        }
        
        private async Task<RecommendationsGetModel> GetRecommendationsGetModel(long userId, RecommendationRequest request)
        {
            MovieContext context = new MovieContext();
            RecommendationRequestDao dao = await context.Set<RecommendationRequestDao>().SingleOrDefaultAsync(rec => rec.UserId == userId);
            List<ReviewRecommendation> reviewRecommendations = await context.Set<ReviewRecommendation>().Where(r => r.UserId == userId).ToListAsync();
            if (dao == null)
            {
                return new RecommendationsGetModel()
                {
                    UserId = userId,
                    Recommendations = new List<ReviewRecommendation>(),
                    RequestPerSession = 1,
                    TopRatedRecommendationPageNumber = 0
                };
            }
            else
            {
                RecommendationsGetModel getModel = dao.MapToGetRecommendationsModel(reviewRecommendations);
                if (getModel.Recommendations == null)
                {
                    getModel.Recommendations = new List<ReviewRecommendation>();
                }
                getModel.RequestPerSession = request.RequestNumber;
                return getModel;
            }
        }

        private async Task<List<MovieDao>> GetRecommendations(RecommendationsGetModel recommendationsGetModel, long userId)
        {
            MovieContext context = new MovieContext();
            if (recommendationsGetModel.RequestPerSession == 1)
            {
                List<ReviewRecommendation> reviews = await GetReviewRecommendations(userId);
                
                recommendationsGetModel.Recommendations = recommendationsGetModel.Recommendations.Concat(reviews).ToList();
                return await GetMoviesFromReviewRecommendations(recommendationsGetModel);
            }
            return await GetTopRated(recommendationsGetModel);
        }

        private async Task<List<MovieDao>> GetMoviesFromReviewRecommendations(RecommendationsGetModel recommendationsGetModel)
        {
            if (recommendationsGetModel.Recommendations.Count == 0)
                return await GetTopRated(recommendationsGetModel);
            List<MovieDao> recommendations = new List<MovieDao>();
            for(int i = recommendationsGetModel.Recommendations.Count()-1; i >= Math.Max(recommendationsGetModel.Recommendations.Count()-4, 0); --i)
            {
                ++recommendationsGetModel.Recommendations[i].NumberOfPagesShown;
                var tmdbIds = await GetSimilarMoviesTmdbIds(recommendationsGetModel.Recommendations[i]);
                
                if(tmdbIds != null)
                {
                    foreach(TmdbIdModel movieId in tmdbIds)
                    {
                        recommendations.Add(await _movieService.GetMovieRecommendation(movieId.id));
                    }
                }
                
            }
            await SaveRecommendationsGetModel(recommendationsGetModel);
            return recommendations;
        }

        public async Task<List<TmdbIdModel>> GetSimilarMoviesTmdbIds(ReviewRecommendation reviewRecommendation)
        {
            var response = await client.GetAsync($"https://api.themoviedb.org/3/movie/{reviewRecommendation.MovieTmdbId}/similar?api_key={ApiKey}&language=en-US&page={reviewRecommendation.NumberOfPagesShown}");
            var content = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<TmdbSearchResult>(content);
            
            return model.results;
        }

        private async Task<List<ReviewRecommendation>> GetReviewRecommendations(long userId)
        {
            MovieContext context = new MovieContext();
            List<ReviewDao> reviews = context.Set<ReviewDao>()
                    .Where(review => review.AuthorId == userId && review.NumberOfStars == 5)
                    .OrderBy(r => r.Id)
                    .ToList();

            reviews.Concat(context.Set<ReviewDao>()
               .Where(review => review.AuthorId == userId && review.NumberOfStars == 4)
               .OrderBy(r => r.Id)
               .ToList());

            return (await ReviewsToReviewRecommendation(reviews));
        }

        private async Task<List<ReviewRecommendation>> ReviewsToReviewRecommendation(List<ReviewDao> reviews) {
            MovieContext context = new MovieContext();
            List<ReviewRecommendation> toReturn = new List<ReviewRecommendation>();

            for(int i = 0; i < reviews.Count; ++i)
            {
                var movie = await context.Set<MovieDao>().FindAsync(reviews[i].DiscussableId);
                if (movie == null)
                {
                    i -= 1;
                    reviews.RemoveAt(i+1);
                }
                else
                {
                    toReturn.Add(new ReviewRecommendation()
                    {
                        ReviewId = reviews[i].Id,
                        UserId = reviews[i].AuthorId,
                        MovieImdbId = movie.ImdbId,
                        HasMore = true,
                        NumberOfPagesShown = 0,
                        MovieTmdbId = movie.TmdbId
                    });
                }
            }
            return toReturn;
        }

        private async Task<List<MovieDao>> GetTopRated(RecommendationsGetModel recommendationsGetModel)
        {
            var response = await client.GetAsync($"https://api.themoviedb.org/3/movie/top_rated?api_key={ApiKey}&language=en-US&page={++recommendationsGetModel.TopRatedRecommendationPageNumber}");
            var content = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<TmdbSearchResult>(content);
            if(recommendationsGetModel.UserId != 0)
                await SaveRecommendationsGetModel(recommendationsGetModel);
            return await GetTopRatedMovies(model);
        }

        private async Task<List<MovieDao>> GetTopRatedMovies(TmdbSearchResult model)
        {
            if (model == null || model.results == null) return new List<MovieDao>();
            var toReturn = model.results.Select(r => _movieService.GetMovieRecommendation(r.id)).ToList();
            await Task.WhenAll(toReturn);
            return toReturn.Select(x => x.Result).ToList();
        }

        private async Task SaveRecommendationsGetModel(RecommendationsGetModel getModel)
        {
            using MovieContext context = new MovieContext();
            var existingDao = await context.Set<RecommendationRequestDao>().SingleOrDefaultAsync(r => r.UserId == getModel.UserId);
            if(existingDao != null)
            {
                existingDao.TopRatedRecommendationPage = getModel.TopRatedRecommendationPageNumber;
                context.Set<RecommendationRequestDao>().Update(existingDao);
            } else
            {
                RecommendationRequestDao dao = new RecommendationRequestDao();
                dao.FromGetRecommendationsModel(getModel);
                context.Set<RecommendationRequestDao>().Add(dao);
            }

            var existingRecommendations = (await context.Set<ReviewRecommendation>().Where(r => r.UserId == getModel.UserId).ToListAsync());
            var existingRecommendationsId = existingRecommendations.Select(e => e.ReviewId);

            var toUpdateRecommendations = getModel.Recommendations.Where(r => existingRecommendationsId.Contains(r.ReviewId));
            var toAddRecommendations = getModel.Recommendations.Where(r => !existingRecommendationsId.Contains(r.ReviewId));
                        
            foreach(ReviewRecommendation existingRecommendation in existingRecommendations)
            {
                foreach(ReviewRecommendation recommendation in toUpdateRecommendations)
                {
                    if(existingRecommendation.ReviewId == recommendation.ReviewId)
                    {
                        existingRecommendation.NumberOfPagesShown = recommendation.NumberOfPagesShown;
                        existingRecommendation.HasMore = recommendation.HasMore;
                        context.Set<ReviewRecommendation>().Update(existingRecommendation);
                    }
                }
            }

            foreach (ReviewRecommendation recommendation in toAddRecommendations)
            {
                context.Set<ReviewRecommendation>().Add(recommendation);
            }

            await context.SaveChangesAsync();
        }
    }
}
