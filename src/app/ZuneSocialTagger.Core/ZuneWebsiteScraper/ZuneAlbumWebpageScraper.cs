using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    /// <summary>
    /// This class assumes valid input from a webpage and will fail to load on an invalid webpage
    /// </summary>
    public class ZuneAlbumWebpageScraper
    {
        private readonly HtmlDocument _document;

        public ZuneAlbumWebpageScraper(string page)
        {
            _document = new HtmlDocument();
            _document.LoadHtml(page);

            VerifyThatWebpageIsAValidZuneWebpage();
        }

        private void VerifyThatWebpageIsAValidZuneWebpage()
        {
            XPathNavigator xPathNavigator = _document.CreateNavigator();

            XPathNavigator titleNode = xPathNavigator.SelectSingleNode("//head/title");

            if (!titleNode.InnerXml.ToLower().Contains("zune.net"))
                throw new WebpageParseException("could not identify this webpage as being a zune.net webpage");
        }

        public Guid ScrapeAlbumMediaID()
        {
            return GetAlbumMediaIdFromMediaInfoAttribute(ScrapeAttribute("_albumHeader", "div/a", "mediainfo"));
        }

        public Guid ScrapeAlbumArtistID()
        {
            return GetAlbumArtistIDFromFanClubAttribute(ScrapeAttribute("_artistHeader", "div/ul", "id"));
        }

        public IEnumerable<Song> GetSongTitleAndIDs()
        {
            HtmlNodeCollection collection;

            try
            {
                HtmlNode node = _document.GetElementbyId("_albumSongs");
                //we are selecting all ul nodes with a class attributeId and a li child with a media info attributeId
                 collection = node.SelectNodes("ul[@class='SongWithOrdinals ']/li[@mediainfo]");
            }
                //TODO: make this exception less generic
            catch (NullReferenceException){throw new WebpageParseException("could not read the song list from the webpage");}


            return collection.Select(nodeCollection => GetIDAndSongNameFromMediaInfoAttribute(nodeCollection.Attributes["mediainfo"].Value));
        }

        public string ScrapeAlbumArtist()
        {
            HtmlNode node = _document.GetElementbyId("_albumHeader");
            HtmlNode albumArtistNode = node.SelectSingleNode("div/ul/li[@class='GeneralMetaData GreyLinkV2 Artist']/a");

            return TrimCarriageReturnsAndBlankSpacesAtStartOfString(albumArtistNode.InnerText);
        }

        public string ScrapeAlbumTitle()
        {
            HtmlNode node = _document.GetElementbyId("_albumHeader");
            HtmlNode albumTitleNode = node.SelectSingleNode("div/ul/li/ul/li/h5");


            //TODO: refactor these three methods

            return TrimCarriageReturnsAndBlankSpacesAtStartOfString(albumTitleNode.InnerText);
        }

        public int ScrapeAlbumReleaseYear()
        {
            HtmlNode node = _document.GetElementbyId("_albumHeader");
            HtmlNode albumReleaseYearNode = node.SelectSingleNode("div/ul/li[@class='GeneralMetaData ReleaseYear']");

            return Convert.ToInt32(TrimCarriageReturnsAndBlankSpacesAtStartOfString(albumReleaseYearNode.InnerText).Substring(9));
        }


        public string ScrapeAlbumArtworkUrl()
        {
            HtmlNode node = _document.GetElementbyId("_albumHeader");
            HtmlNode albumReleaseYearNode = node.SelectSingleNode("div/a/img[@class='LargeImage jsImage']");

            return albumReleaseYearNode.Attributes["src"].Value;
        }

        private static string TrimCarriageReturnsAndBlankSpacesAtStartOfString(string input)
        {
            return input.Replace("\n", "").Replace("\r", "").TrimStart();
        }

        /// <summary>
        /// Gets the specified attributeId value from the provided values
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="xpathQuery"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        private string ScrapeAttribute(string elementId, string xpathQuery, string attributeId)
        {
            try
            {
                HtmlNode node = _document.GetElementbyId(elementId);
                HtmlNode singleNode = node.SelectSingleNode(xpathQuery);
                HtmlAttribute attribute = singleNode.Attributes[attributeId];

                return attribute.Value;
            }
            catch (NullReferenceException)
            {
                //TODO: make the exception less generic
                throw new WebpageParseException("could not get the requested attributeId");
            }
        }

        /// <summary>
        /// Extracts the AlbumArtistID from a href attributeId
        /// </summary>
        /// <param name="attributeString">Should look like this: FanClub00710a00-0600-11db-89ca-0019b92a3933</param>
        /// <returns></returns>
        private static Guid GetAlbumArtistIDFromFanClubAttribute(string attributeString)
        {
            return new Guid(attributeString.Substring(attributeString.Length - 36));
        }

        private static Song GetIDAndSongNameFromMediaInfoAttribute(string attributeString)
        {
            return GetMediaInfoAttributeData(attributeString, "#song#");
        }

        private static Guid GetAlbumMediaIdFromMediaInfoAttribute(string attributeString)
        {
            return GetMediaInfoAttributeData(attributeString, "#album#").Guid;
        }

        /// <summary>
        /// Splits a mediainfo attributeId into a keypair
        /// </summary>
        /// <param name="attributeString">Should look like: 41b9f201-0100-11db-89ca-0019b92a3933#song#Hari Kari
        ///                               Should look like: 37b9f201-0100-11db-89ca-0019b92a3933#album#Ignore The Ignorant</param>
        /// <returns></returns>
        private static Song GetMediaInfoAttributeData(string attributeString, string splitOn)
        {
            var regex = new Regex(splitOn);

            //Should only ever split into 2 anyway so the 2 isnt really neccessary

            string[] split = regex.Split(attributeString, 2);

            return new Song {Guid = new Guid(split[0]), Title = split[1]};
        }
    }
}