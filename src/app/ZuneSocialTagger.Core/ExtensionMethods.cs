using System;
using System.Collections.Generic;

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
            int startIndex = urn.LastIndexOf(':') + 1;

            if (startIndex == 0) return Guid.Empty;

            Guid result;
            try
            {
                result = new Guid(urn.Substring(startIndex));
            }
            catch (Exception)
            {
                result = Guid.Empty;
            }

            return result;
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