using System;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests
{
    public class ArtistSearchTestHarness
    {
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Search for artist:");
                string artist = Console.ReadLine();

                foreach (var result in AlbumSearch.SearchFor(artist))
                {
                    Console.WriteLine("Artist: {0}, Album: {1}",result.Artist,result.Title);
                }
            }
        }
    }
}