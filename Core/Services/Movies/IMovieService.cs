﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.Movies.Models;

namespace Core.Services.Movies
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListModal>> SearchList(string text);
        Task<Movie> GetMovieDetails(string ImdbId);
    }
}