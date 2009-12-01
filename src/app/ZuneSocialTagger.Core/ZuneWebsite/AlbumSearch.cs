using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static IEnumerable<Album> SearchFor(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}",Urls.Album, searchString);

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
                    XElement artistElement = item.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist", Urls.Schema).First();

                    XElement artistTitleElement = artistElement.Elements().Where(x => x.Name.LocalName == "name").First();

                    Guid imageGuid =
                        item.ElementExtensions.ReadElementExtensions<XElement>("image", Urls.Schema)
                            .First().Value.ExtractGuidFromUrnUuid();


                    string imagePath = String.Format("{0}{1}?width=60&height=60",Urls.Image, imageGuid);
                

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
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName,Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }
    }
}