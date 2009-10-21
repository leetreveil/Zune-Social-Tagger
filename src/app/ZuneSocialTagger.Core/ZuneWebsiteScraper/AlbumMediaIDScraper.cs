using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    /// <summary>
    /// This class assumes valid input from a webpage and will fail on an invalid webpage
    /// </summary>
    public class AlbumMediaIDScraper
    {
        private readonly HtmlDocument _document;

        public AlbumMediaIDScraper(string page)
        {
            _document = new HtmlDocument();

            HtmlDocument document = _document;

            document.LoadHtml(page);
        }

        public Dictionary<string, string> Scrape()
        {
            HtmlNode node = _document.GetElementbyId("_albumSongs");

            HtmlNodeCollection collection = node.SelectNodes("ul[@class='SongWithOrdinals ']/li[@mediainfo]");

            var dict = new Dictionary<string, string>();

            foreach (var nodeCollection in collection)
            {
                HtmlAttribute htmlAttribute = nodeCollection.Attributes["mediainfo"];

                string[] idAndSongName = GetIDAndSongNameFromMediaInfoAttribute(htmlAttribute.Value);

                dict.Add(idAndSongName[1],idAndSongName[0]);
            }

            return dict;
        }

        /// <summary>
        /// Splits a mediainfo attribute into a keypair
        /// </summary>
        /// <param name="attributeString">Should look like this: 41b9f201-0100-11db-89ca-0019b92a3933#song#Hari Kari</param>
        /// <returns></returns>
        public static string[] GetIDAndSongNameFromMediaInfoAttribute(string attributeString)
        {
            var regex = new Regex("#song#");

            //Should only ever split into 2 anyway so the 2 isnt really neccessary

            string[] split = regex.Split(attributeString, 2);

            return split;
        }
    }
}