using System;
using System.Text.RegularExpressions;

namespace ZuneSocialTagger.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Trims /r/n feeds in strings 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimCarriageReturns(this string input)
        {
            return input.Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Extracts guids from strings which may or may not have other data in, example string : FanClub00710a00-0600-11db-89ca-0019b92a3933
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ExtractGuid(this string str)
        {
            Regex regex = new Regex(".*?([A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12})",
                                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (regex.IsMatch(str) && regex.Match(str).Success)
                return new Guid(regex.Match(str).Groups[1].Value);

            throw new FormatException("could not extract a guid from string");
        }
    }
}