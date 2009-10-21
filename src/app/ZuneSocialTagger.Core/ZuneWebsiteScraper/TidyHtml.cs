using System;
using System.IO;
using System.Text;
using TidyNet;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public static class TidyHtml
    {
        public static string Clean(string input)
        {
            var tidy = new Tidy();

            tidy.Options.DropEmptyParas = true;
            tidy.Options.FixComments = true;
            tidy.Options.Word2000 = true;
            tidy.Options.QuoteAmpersand = true;
            tidy.Options.QuoteNbsp = true;
            tidy.Options.WrapPhp = true;
            tidy.Options.WrapSection = true;
            tidy.Options.CharEncoding = CharEncoding.UTF8;
            tidy.Options.DocType = DocType.Auto;
            tidy.Options.FixBackslash = true;

            TidyMessageCollection tmc = new TidyMessageCollection();

            using (var inStream = new MemoryStream(Encoding.UTF8.GetBytes(input)) { Position = 0 })
            using (var outStream = new MemoryStream())
            {
                tidy.Parse(inStream, outStream, tmc);

                return Encoding.UTF8.GetString(outStream.ToArray());
            }
        }
    }
}