using System;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// urn:uuid:c14c4e00-0300-11db-89ca-0019b92a3933
        /// </summary>
        /// <param name="urn"></param>
        /// <returns>c14c4e00-0300-11db-89ca-0019b92a3933</returns>
        public static Guid ExtractGuidFromUrnUuid(this string urn)
        {
            return new Guid(urn.Substring(urn.LastIndexOf(':') + 1));
        }

        /// <summary>
        /// Verify's that a list of song guids are valid and returns false if any are invalid
        /// </summary>
        /// <param name="songGuids"></param>
        /// <returns></returns>
        public static bool AreAllValid(this IEnumerable<Track> songGuids)
        {
            //empty list then its invalid
            if (songGuids == null || songGuids.Count() == 0)
                return false;

            //if any guids are not valid
            return !songGuids.Any(guid => !guid.IsValid());
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var enumerable1 in enumerable)
            {
                action(enumerable1);
            }
        }
    }
}