using System;
using System.Collections.Generic;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;

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

                IEnumerable<Album> result = AlbumSearch.SearchFor(artist);

                foreach (var album in result)
                {
                    Console.WriteLine("Artist: {0}, Album: {1}", album.Artist, album.Title);
                }
            }
        }
    }
}