using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;
using System.Diagnostics;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class ArtistSearch
    {
        public static IEnumerable<WebArtist> SearchFor(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}", Urls.Artist, searchString);

            try
            {
                return ReadArtistsFromXmlDocument(XmlReader.Create(searchUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new List<WebArtist>();
        }

        public static void SearchForAsync(string searchString,Action<IEnumerable<WebArtist>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchFor(searchString).ToList()));
        }

        private static IEnumerable<WebArtist> ReadArtistsFromXmlDocument(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    yield return new WebArtist
                                     {
                                         Name = item.Title.Text,
                                         Id = item.Id.ExtractGuidFromUrnUuid()
                                     };
                }
            }
        }
    }
}