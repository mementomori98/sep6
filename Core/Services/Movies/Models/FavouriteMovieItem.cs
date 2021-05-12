using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Movies.Models
{
    public class FavouriteMovieItem
    {
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Poster { get; set; }
        public string Plot { get; set; }
    }
}
