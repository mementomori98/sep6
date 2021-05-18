using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utils
{
    public static class LinqExtensions
    {

        public static TType MinFallback<TType, TSource>(this IEnumerable<TSource> source, Func<TSource, TType> selector, TType fallback = default)
        {
            if (!source?.Any() ?? true)
                return fallback;
            return source.Min(selector);
        }
        
        public static TType MaxFallback<TType, TSource>(this IEnumerable<TSource> source, Func<TSource, TType> selector, TType fallback = default)
        {
            if (!source?.Any() ?? true)
                return fallback;
            return source.Max(selector);
        }
        
    }
}