using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Extensions
{
    public static class PublicExtension
    {
        public static string AddBaseUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(url) == false)
            {
                if (Setting.IsDevelopment)
                {
                    return "https://localhost:5001" + url;
                }
                else
                {
                    return "https://puzzlearchitect.ir" + url;
                }
            }

            return null;
        }
    }
}
