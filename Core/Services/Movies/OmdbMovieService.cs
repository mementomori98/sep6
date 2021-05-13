﻿using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Services.Movies.Models;

namespace Core.Services.Movies
{
    public class OmdbMovieService : IMovieService
    {
        public async Task<IEnumerable<MovieListModal>> SearchList(string text)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"http://www.omdbapi.com/?apikey=720a2f69&s={text}");
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (!dict.ContainsKey("Search"))
                return new MovieListModal[0];
            var array = dict["Search"];
            return JsonSerializer.Deserialize<IEnumerable<MovieListModal>>(array.ToString());
        }
    }
}