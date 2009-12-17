using System;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.IntegrationTests
{
    public class AlbumWebpageScraperTestHarness
    {
        #region Urls

        private static string[] _webpages = new[]
            {
               "http://catalog.zune.net/v3.0/en-US/music/album/95dc1002-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/37b9f201-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/eb54f601-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/6f621500-0400-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/7510d300-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/febc1800-0400-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/4f66ff01-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/355c0802-0100-11db-89ca-0019b92a3933"
                ,
                "http://catalog.zune.net/v3.0/en-US/music/album/fbed1700-0400-11db-89ca-0019b92a3933/"
            };

        #endregion

        [STAThread]
        public static void Main()
        {
            Console.WriteLine("downloading {0} zune album webpages...", _webpages.Length);

            foreach (var url in _webpages)
            {
                var scraper = new AlbumDocumentReader(url);
                Album result = scraper.Read();

                Console.WriteLine("ZuneAlbumMediaID: {0}", result.AlbumMediaID);
                Console.WriteLine("Album Artist: {0}", result.Artist);
                Console.WriteLine("Title: {0}", result.Title);
                Console.WriteLine("Release Year: {0}", result.ReleaseYear);
                Console.WriteLine("Artwork url: {0}", result.ArtworkUrl);

                Console.WriteLine("Does this page have the minimum required info to link: {0}", result.IsValid);
                Console.WriteLine("");
            }

            Console.WriteLine("finished...");
            Console.ReadLine();
        }
    }
}