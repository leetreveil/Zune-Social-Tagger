using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Diagnostics;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearchScraper
    {
        private readonly HtmlDocument _document;

        public AlbumSearchScraper(string pageData)
        {
            _document = new HtmlDocument();
            _document.LoadHtml(pageData);
        }

        public IEnumerable<AlbumSearchResult> ScrapeAlbums()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HtmlNode node = _document.DocumentNode;
            HtmlNodeCollection albumList = node.SelectNodes("//div[@class='AlbumList']");

            foreach (var nod in albumList)
            {
                HtmlNode artistNode = nod.SelectSingleNode("a[@class ='Artist']");
                HtmlNode albumLinkNode = nod.SelectSingleNode("a[@href]");

                //substringing artist beacause it has " - " prepended
                yield return
                    new AlbumSearchResult
                        {
                            Artist = artistNode.InnerText.Substring(3),
                            Title = albumLinkNode.InnerText,
                            Url = albumLinkNode.Attributes["href"].Value
                        };
            }
            sw.Stop();

            Console.WriteLine("Time taken to scrape albums: {0}",sw.ElapsedMilliseconds);
        }

        public int ScrapeAlbumCountAcrossAllPages()
        {
            HtmlNode node = _document.DocumentNode;
            HtmlNode singleNode = node.SelectSingleNode("/div[2]/span[2]");

            return GetCountFromSearchSubResult(singleNode.InnerText);
        }

        private static int GetCountFromSearchSubResult(string subResult)
        {
            int result = 0;
            Regex r = new Regex("(\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(subResult);
            if (m.Success)
            {
                bool parseResult = int.TryParse(m.Groups[1].ToString(), out result);

                if (parseResult == false)
                {
                    //TODO: better exception
                    throw new Exception("could not get count!");
                }
            }

            return result;
        }
    }
}