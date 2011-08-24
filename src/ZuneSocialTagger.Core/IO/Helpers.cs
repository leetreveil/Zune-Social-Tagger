using System;
using System.Linq;

namespace ZuneSocialTagger.Core.IO
{
    public static class Helpers
    {
        /// <summary>
        /// Converts Track numbers that are like 1/2 to just 1 or 4/11 to just 4
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns></returns>
        public static int ToTrackNum(this string trackNumber)
        {
            if (String.IsNullOrEmpty(trackNumber)) return 0;

            return trackNumber.Contains('/') 
                ? Convert.ToInt32(trackNumber.Split('/').First()) 
                : Convert.ToInt32(trackNumber);
        }

        /// <summary>
        /// Converts Disc number 1/2 to just one and 2/2 to just 2
        /// </summary>
        /// <param name="discNumber"></param>
        /// <returns></returns>
        public static int ToDiscNum(this string discNumber)
        {
            if (String.IsNullOrEmpty(discNumber)) return 0;

            return discNumber.Contains('/')
                ? Convert.ToInt32(discNumber.Split('/').First())
                : Convert.ToInt32(discNumber);
        }
    }
}
