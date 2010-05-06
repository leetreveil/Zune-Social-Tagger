using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static IEnumerable<WebAlbum> SearchFor(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}", Urls.Album, searchString);

            return ReadFromXmlDocument(XmlReader.Create(searchUrl));
        }

        public static void SearchForAsync(string searchString, Action<IEnumerable<WebAlbum>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchFor(searchString).ToList()));
        }

        public static IEnumerable<WebAlbum> GetAlbumsFromArtistGuid(Guid guid)
        {
            var artistAlbumsUrl = String.Format("{0}{1}/albums", Urls.Artist, guid);

            return ReadFromXmlDocument(XmlReader.Create(artistAlbumsUrl));
        }

        public static IEnumerable<WebAlbum> ReadFromXmlDocument(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    yield return new WebAlbum
                         {
                             Title = item.Title.Text,
                             AlbumMediaId = item.Id.ExtractGuidFromUrnUuid(),
                             Artist = item.GetArtist(),
                             ArtworkUrl = item.GetArtworkUrl(),
                             ReleaseYear = item.GetReleaseYear()
                         };
                }
            }
        }
    }
}