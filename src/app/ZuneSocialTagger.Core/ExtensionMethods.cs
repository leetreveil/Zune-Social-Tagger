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
    }
}