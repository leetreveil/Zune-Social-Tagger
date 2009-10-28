using System;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.IntegrationTests
{
    public class AlbumSearchTestHarness
    {
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Search for artist:");
                string artist = Console.ReadLine();

                AlbumSearch.SearchForAsync(artist,
                                           result =>
                                           result.ForEach(
                                               x => Console.WriteLine("Artist: {0}, Album: {1}", x.Artist, x.Title)));
            }
        }
    }
}