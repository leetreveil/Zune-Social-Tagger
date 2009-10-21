using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    /// <summary>
    /// This class assumes valid input from a webpage and will fail to load on an invalid webpage
    /// </summary>
    public class AlbumMediaIDScraper
    {
        private readonly HtmlDocument _document;

        public AlbumMediaIDScraper(string page)
        {
            _document = new HtmlDocument();
            _document.LoadHtml(page);
        }

        public Guid ScrapeAlbumMediaID()
        {
            return GetAlbumMediaIdFromMediaInfo(ScrapeAttribute("_albumHeader", "div/a", "mediainfo"));
        }

        public Guid ScrapeAlbumArtistID()
        {
            return GetAlbumArtistIDFromFanClubAttribute(ScrapeAttribute("_artistHeader", "div/ul", "id"));
        }

        public IEnumerable<KeyValuePair<string,Guid>> GetSongTitleAndIDs()
        {
            HtmlNodeCollection collection;

            try
            {
                HtmlNode node = _document.GetElementbyId("_albumSongs");
                //we are selecting all ul nodes with a class attribute and a li child with a media info attribute
                 collection = node.SelectNodes("ul[@class='SongWithOrdinals ']/li[@mediainfo]");
            }
            catch (Exception){throw new Exception("problem with the html file");}


            foreach (var nodeCollection in collection)
                yield return GetIDAndSongNameFromMediaInfoAttribute(nodeCollection.Attributes["mediainfo"].Value);
        }

        private string ScrapeAttribute(string elementId, string singleNode, string attribute)
        {
            try
            {
                HtmlNode node = _document.GetElementbyId(elementId);
                HtmlNode singleNodex = node.SelectSingleNode(singleNode);
                HtmlAttribute attributex = singleNodex.Attributes[attribute];

                return attributex.Value;
            }
            catch (Exception)
            {
                //TODO: make the exception less generic
                throw new Exception("problem with the html file");
            }
        }

        /// <summary>
        /// Extracts the AlbumArtistID from a href attribute
        /// </summary>
        /// <param name="attributeString">Should look like this: FanClub00710a00-0600-11db-89ca-0019b92a3933</param>
        /// <returns></returns>
        private static Guid GetAlbumArtistIDFromFanClubAttribute(string attributeString)
        {
            return new Guid(attributeString.Substring(attributeString.Length - 36));
        }

        private static KeyValuePair<string, Guid> GetIDAndSongNameFromMediaInfoAttribute(string attributeString)
        {
            return GetMediaInfoAttributeData(attributeString, "#song#");
        }

        private static Guid GetAlbumMediaIdFromMediaInfo(string attributeString)
        {
            return GetMediaInfoAttributeData(attributeString, "#album#").Value;
        }

        /// <summary>
        /// Splits a mediainfo attribute into a keypair
        /// </summary>
        /// <param name="attributeString">Should look like: 41b9f201-0100-11db-89ca-0019b92a3933#song#Hari Kari
        ///                               Should look like: 37b9f201-0100-11db-89ca-0019b92a3933#album#Ignore The Ignorant</param>
        /// <returns></returns>
        private static KeyValuePair<string,Guid> GetMediaInfoAttributeData(string attributeString, string splitOn)
        {
            var regex = new Regex(splitOn);

            //Should only ever split into 2 anyway so the 2 isnt really neccessary

            string[] split = regex.Split(attributeString, 2);

            return new KeyValuePair<string, Guid>(split[1], new Guid(split[0]));
        }
    }
}