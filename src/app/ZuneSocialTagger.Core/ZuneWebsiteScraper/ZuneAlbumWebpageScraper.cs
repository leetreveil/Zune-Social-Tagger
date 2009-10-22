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
        private string _albumHeaderNodeId = "_albumHeader";

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
            return
                GetAlbumMediaIdFromMediaInfoAttribute(
                    _document.GetNode("_albumHeader", "div/a").Attributes["mediainfo"].Value);
        }

        public Guid ScrapeAlbumArtistID()
        {
            return
                GetAlbumArtistIDFromFanClubAttribute(_document.GetNode("_artistHeader", "div/ul").Attributes["id"].Value);
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
            catch (NullReferenceException)
            {
                throw new WebpageParseException("could not read the song list from the webpage");
            }


            return
                collection.Select(
                    nodeCollection =>
                        GetIDAndSongNameFromMediaInfoAttribute(nodeCollection.Attributes["mediainfo"].Value));
        }

        public string ScrapeAlbumArtist()
        {
            return
                _document.GetNode(_albumHeaderNodeId, "div/ul/li[@class='GeneralMetaData GreyLinkV2 Artist']/a").
                    InnerText.TrimCarriageReturns().TrimStart();
        }

        public string ScrapeAlbumTitle()
        {
            return
                _document.GetNode(_albumHeaderNodeId, "div/ul/li/ul/li/h5").InnerText.TrimCarriageReturns().TrimStart();
        }

        public int ScrapeAlbumReleaseYear()
        {
            return Convert.ToInt32(_document.GetNode(_albumHeaderNodeId,
                                                         "div/ul/li[@class='GeneralMetaData ReleaseYear']").InnerText.
                                                             TrimCarriageReturns().TrimStart().Substring(9));
        }


        public string ScrapeAlbumArtworkUrl()
        {
            return
                _document.GetNode(_albumHeaderNodeId, "div/a/img[@class='LargeImage jsImage']").Attributes["src"].Value;
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