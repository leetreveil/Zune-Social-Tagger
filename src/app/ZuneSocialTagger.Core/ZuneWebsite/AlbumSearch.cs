using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static void SearchForAsync(string searchString, Action<IEnumerable<AlbumSearchResult>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchFor(searchString)));
        }

        public static IEnumerable<AlbumSearchResult> SearchFor(string searchString)
        {
            string searchUrl = String.Format("http://catalog.zune.net/v3.0/en-US/music/album?q={0}", searchString);

            XmlReader reader = XmlReader.Create(searchUrl);

            return ReadFromXmlDocument(reader);
        }

        public static IList<AlbumSearchResult> ReadFromXmlDocument(XmlReader reader)
        {
            var tempList = new List<AlbumSearchResult>();

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    //Console.WriteLine(item.Title.Text);

                    XElement artistElement = item.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist", "http://schemas.zune.net/catalog/music/2007/10").First();

                    //XElement idElement = artistElement.Elements().Where(x => x.Name.LocalName == "id").First();
                    XElement artistTitleElement = artistElement.Elements().Where(x => x.Name.LocalName == "name").First();

                    tempList.Add(new AlbumSearchResult { Title = item.Title.Text, Guid = item.Id.ExtractGuidFromUrnUuid(),Artist = artistTitleElement.Value});
                    //yield return new AlbumSearchResult{Title = item.Title.Text,Guid = ExtractGuidFromUrnUuid(item.Id)};
                }
            }

            return tempList;
        }
    }
}