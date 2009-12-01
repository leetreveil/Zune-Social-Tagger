using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static void SearchForAsync(string searchString, Action<IEnumerable<Album>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchFor(searchString)));
        }

        public static IEnumerable<Album> SearchFor(string searchString)
        {
            string searchUrl = String.Format("http://catalog.zune.net/v3.0/en-US/music/album?q={0}", searchString);

            XmlReader reader = XmlReader.Create(searchUrl);

            return ReadFromXmlDocument(reader);
        }

        public static IList<Album> ReadFromXmlDocument(XmlReader reader)
        {
            var tempList = new List<Album>();

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    XElement artistElement = item.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist", "http://schemas.zune.net/catalog/music/2007/10").First();

                    XElement artistTitleElement = artistElement.Elements().Where(x => x.Name.LocalName == "name").First();

                    Guid imageGuid =
                        item.ElementExtensions.ReadElementExtensions<XElement>("image","http://schemas.zune.net/catalog/music/2007/10")
                            .First().Value.ExtractGuidFromUrnUuid();


                    string imagePath = String.Format("http://image.catalog.zune.net/v3.0/image/{0}?width=60&height=60", imageGuid);
                

                    tempList.Add(new Album { Title = item.Title.Text, 
                                                         AlbumMediaID = item.Id.ExtractGuidFromUrnUuid(),
                                                         Artist = artistTitleElement.Value,
                                                         ArtworkUrl = imagePath,
                                                         ReleaseYear = GetReleaseYear(item) 
                                                        });
                }
            }

            return tempList;
        }

        private static int? GetReleaseYear(SyndicationItem feed)
        {
            //TODO: refactor this class and AlbumDocumentReader into one single class.
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year : (int?)null;
        }

        private static XElement GetElement(SyndicationItem feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName,
                                                           "http://schemas.zune.net/catalog/music/2007/10");

            return elements.Count > 0 ? elements.First() : null;
        }
    }
}