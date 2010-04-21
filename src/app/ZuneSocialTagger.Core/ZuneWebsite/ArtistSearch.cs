using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class ArtistSearch
    {
        public static IEnumerable<Artist> SearchFor(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}", Urls.Artist, searchString);

            XmlReader reader = XmlReader.Create(searchUrl);

            return ReadArtistsFromXmlDocument(reader);
        }

        private static IEnumerable<Artist> ReadArtistsFromXmlDocument(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    yield return new Artist
                                     {
                                         Name = item.Title.Text,
                                         Id = item.Id.ExtractGuidFromUrnUuid()
                                     };
                }
            }
        }
    }
}