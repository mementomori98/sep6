using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations.Models
{
    public class TmdbFindByExternalIdMovieModel
    {
        public List<TmdbIdModel> movie_results { get; set; }
    }
}
