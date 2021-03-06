using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Models;
using Core.Domain.DiscussionItems.Models;
using Core.Domain.Movies.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Domain.Movies
{
    public class MovieService : IMovieService, IMovieRecommendationService
    {
        private static HttpClient client = new HttpClient();
        private static string ApiKey = "720a2f69";

        public async Task<MovieModel> GetMovieDetails(string imdbId)
        {
            return await GetMovieDetails(imdbId, await GetTmdbId(imdbId));
        }

        public async Task<MovieDao> GetMovieRecommendation(long tmdbId)
        {
            MovieContext context = new MovieContext();
            var movie = context.Set<MovieDao>()
                .SingleOrDefault(m => m.TmdbId == tmdbId);
            if (movie != null) return movie;
            else
            {
                var movieDetails = await GetMovieDetails(tmdbId);
                return MovieDao.MapFromModel(movieDetails);
            }
        }

        private async Task<MovieModel> GetMovieDetails(string ImdbId, long tmdbId)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&i={ImdbId}&plot=full");
            var content = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<MovieApiModel>(content);
            model.TmdbId = tmdbId;
            
            return await Sync(model);
        }

        private async Task<string> GetImdbId(long tmdbId)
        {
            var response = await client.GetAsync($"https://sep6movies-statiscics.herokuapp.com/imdb_id/{tmdbId}");
            string imdbIdJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<string>(imdbIdJson);
        }

        private async Task<long> GetTmdbId(string imdbId)
        {
            var response = await client.GetAsync($"https://sep6movies-statiscics.herokuapp.com/tmdb_id/{imdbId}");
            string tmdbIdJson = await response.Content.ReadAsStringAsync();
            
            return long.Parse(tmdbIdJson.Replace("\"", ""));
            
        }

        private async Task<MovieModel> GetMovieDetails(long tmdbId)
        {
            return await GetMovieDetails(await GetImdbId(tmdbId), tmdbId);
        }

        public async Task<IEnumerable<MovieListModel>> SearchList(string text)
        {
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey={ApiKey}&s={text}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (!dict.ContainsKey("Search"))
                return new MovieListModel[0];
            var array = dict["Search"];
            var list = JsonSerializer.Deserialize<IEnumerable<MovieListApiModel>>(array.ToString());
            return list.Select(m => new MovieListModel
            {
                ImageUrl = m.Poster,
                Title = m.Title,
                ImdbId = m.ImdbId,
                Type = m.Type,
                Year = m.Year
            });
        }

        private async Task<MovieModel> Sync(MovieApiModel model)
        {
            await using var context = new MovieContext();
            var movie = context.Set<MovieDao>()
                .SingleOrDefault(m => m.ImdbId == model.imdbID);
            if (movie == null)
            {
                var entry = await context.Set<MovieDao>().AddAsync(new MovieDao
                {
                    ImdbId = model.imdbID,
                    Title = model.Title,
                    Year = model.Year,
                    ImageUrl = model.Poster,
                    TmdbId = model.TmdbId,
                    Director = model.Director,
                    Runtime = model.Runtime
                });
                await context.SaveChangesAsync();
                movie = entry.Entity;
            }
            else
            {
                movie.Year = model.Year;
                movie.ImageUrl = model.Poster;
                movie.Title = model.Title;
                movie.ImdbId = model.imdbID;
                context.Update(movie);
                await context.SaveChangesAsync();
            }

            return new MovieModel
            {
                Id = movie.Id,
                ImdbId = model.imdbID,
                TmdbId = model.TmdbId,
                Actors = model.Actors.Split(",").Select(s => s.Trim()),
                Director = model.Director,
                Genres = model.Genre.Split(",").Select(s => s.Trim()),
                Plot = model.Plot,
                ImageUrl = model.Poster,
                Runtime = model.Runtime,
                Title = model.Title,
                Type = model.Type,
                Year = model.Year,
                DiscusionItems = new List<DiscussionItemModelBase>(),
                Ratings = model.Ratings.Append(new Rating
                {
                    Source = "Imdb",
                    Value = model.imdbRating,
                    Votes = model.imdbVotes == "N/A" ? null : long.Parse(model.imdbVotes.Replace(",", ""))
                })
            };
        }
    }
}