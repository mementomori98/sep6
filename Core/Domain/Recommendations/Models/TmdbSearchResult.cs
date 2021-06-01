using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Recommendations.Models
{
    class TmdbSearchResult
    {
        public int page { get; set; }
        public List<TmdbIdModel> results { get; set; }
    }
}
