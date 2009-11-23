using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    /// <summary>
    /// This class assumes valid input from a webpage and will fail to load on an invalid webpage
    /// </summary>
    public class AlbumWebpageScraper
    {
        private readonly HtmlDocument _document;
        private string _albumHeaderNodeId = "_albumHeader";

        public AlbumWebpageScraper(string pageData)
        {
            _document = new HtmlDocument();
            _document.LoadHtml(pageData);
        }

        public AlbumWebpageScrapeResult Scrape()
        {
            return new AlbumWebpageScrapeResult
                             {
                                 AlbumArtistID = this.ScrapeAlbumArtistID(),
                                 AlbumMediaID = this.ScrapeAlbumMediaID(),
                                 SongTitlesAndMediaID = this.GetSongTitleAndIDs(),
                                 AlbumArtist = this.ScrapeAlbumArtist(),
                                 AlbumTitle = this.ScrapeAlbumTitle(),
                                 AlbumReleaseYear = this.ScrapeAlbumReleaseYear(),
                                 AlbumArtworkUrl = this.ScrapeAlbumArtworkUrl()
                             };
        }

        private Guid ScrapeAlbumMediaID()
        {
            HtmlNode node = _document.GetNodeByIdAndXpath(_albumHeaderNodeId, "div/a");

            return ExtractAGuidFromPage(node, "mediainfo");
        }

        private Guid ScrapeAlbumArtistID()
        {
            HtmlNode node = _document.GetNodeByIdAndXpath("_artistHeader", "div/ul");

            return ExtractAGuidFromPage(node, "id");
        }

        private static Guid ExtractAGuidFromPage(HtmlNode node, string attributeName)
        {
            return node == null ? new Guid() : node.Attributes[attributeName].Value.ExtractGuid();
        }

        private IEnumerable<SongGuid> GetSongTitleAndIDs()
        {
            HtmlNodeCollection collection;
            HtmlNode node = _document.GetElementbyId("_albumSongs");

            if (node == null)
                return new List<SongGuid>();

            //we are selecting all ul nodes with a class attributeId and a li child with a media info attributeId
            collection = node.SelectNodes("ul[@class='SongWithOrdinals ']/li[@mediainfo]");

            return
                collection.Select(htmlNode =>
                                  GetIDAndSongNameFromMediaInfoAttribute(htmlNode.Attributes["mediainfo"].Value));
        }

        private string ScrapeAlbumArtist()
        {
            return this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li[@class='GeneralMetaData GreyLinkV2 Artist']/a");
        }

        private string ScrapeAlbumTitle()
        {
            return this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li/ul/li/h5");
        }

        private int? ScrapeAlbumReleaseYear()
        {
            //the reslease year is extracted from a string like this: Released 2009
            //substring is skipping the first 9 characters
            string releaseYear = this.GetTextFromAlbumHeaderNodeAndClean("div/ul/li[@class='GeneralMetaData ReleaseYear']");

            if (String.IsNullOrEmpty(releaseYear))
                return null;

            return Convert.ToInt32(releaseYear.Substring(9));
        }

        private string ScrapeAlbumArtworkUrl()
        {
            HtmlNode node = _document.GetNodeByIdAndXpath(_albumHeaderNodeId, "div/a/img[@class='LargeImage jsImage']");

            return node == null ? null : node.Attributes["src"].Value;
        }

        private string GetTextFromAlbumHeaderNodeAndClean(string xPath)
        {
            HtmlNode node = _document.GetNodeByIdAndXpath(_albumHeaderNodeId, xPath);

            return node == null ? null : node.InnerText.TrimCarriageReturns().TrimStart();
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