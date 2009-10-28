using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    /// <summary>
    /// This class assumes valid input from a webpage and will fail to load on an invalid webpage
    /// </summary>
    public class AlbumWebpageScraper
    {
        private readonly HtmlDocument _document;
        private const string _albumHeaderNodeId = "_albumHeader";

        public AlbumWebpageScraper(string page)
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
            return _document.GetNodeByIdAndXpath(_albumHeaderNodeId, "div/a").Attributes["mediainfo"].Value.ExtractGuid();
        }

        public Guid ScrapeAlbumArtistID()
        {
            return _document.GetNodeByIdAndXpath("_artistHeader", "div/ul").Attributes["id"].Value.ExtractGuid();
        }

        public IEnumerable<SongGuid> GetSongTitleAndIDs()
        {
            HtmlNodeCollection collection;
            HtmlNode node = _document.GetElementbyId("_albumSongs");

            //we are selecting all ul nodes with a class attributeId and a li child with a media info attributeId
            collection = node.SelectNodes("ul[@class='SongWithOrdinals ']/li[@mediainfo]");

            return
                collection.Select(htmlNode =>
                    GetIDAndSongNameFromMediaInfoAttribute(htmlNode.Attributes["mediainfo"].Value));
        }

        public string ScrapeAlbumArtist()
        {
            return this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li[@class='GeneralMetaData GreyLinkV2 Artist']/a");
        }

        public string ScrapeAlbumTitle()
        {
            return this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li/ul/li/h5");
        }

        public int ScrapeAlbumReleaseYear()
        {
            //the reslease year is extracted from a string like this: Released 2009
            //substring is skipping the first 9 characters
            return
                Convert.ToInt32(
                    this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li[@class='GeneralMetaData ReleaseYear']").Substring
                        (9));
        }

        public string ScrapeAlbumArtworkUrl()
        {
            return
                _document.GetNodeByIdAndXpath(_albumHeaderNodeId, "div/a/img[@class='LargeImage jsImage']").Attributes[
                    "src"].Value;
        }

        private string GetTextFromAlbumHeaderNodeAndClean(string xPath)
        {
            return _document.GetNodeByIdAndXpath(_albumHeaderNodeId, xPath).InnerText.TrimCarriageReturns().TrimStart();
        }

        /// <summary>
        /// Splits a mediainfo attributeId into a SongGuid
        /// </summary>
        /// <param name="attributeString">Should look like: 41b9f201-0100-11db-89ca-0019b92a3933#song#Hari Kari
        ///                               Should look like: 37b9f201-0100-11db-89ca-0019b92a3933#album#Ignore The Ignorant</param>
        /// <returns></returns>
        private static SongGuid GetIDAndSongNameFromMediaInfoAttribute(string attributeString)
        {
            return new SongGuid
                       {
                           Guid = attributeString.ExtractGuid(),
                           Title = attributeString.Substring(attributeString.LastIndexOf('#') + 1)
                       };
        }
    }
}