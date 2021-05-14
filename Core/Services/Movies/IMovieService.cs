using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.Movies.Models;

namespace Core.Services.Movies
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListModel>> SearchList(string text);
        Task<MovieModel> GetMovieDetails(string ImdbId);
    }
}