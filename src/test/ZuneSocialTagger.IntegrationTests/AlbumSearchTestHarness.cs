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
                Console.WriteLine("Search for album and artists:");

                string searchString = Console.ReadLine();

                IEnumerable<Artist> artists = ArtistSearch.SearchFor(searchString);

                Console.WriteLine("---ARTISTS---");
                foreach (var artist in artists)
                {
                    Console.WriteLine("Artist: {0}, Guid: {1}",artist.Name,artist.Id);
                }

                IEnumerable<Album> albums = AlbumSearch.SearchFor(searchString);

                Console.WriteLine("---ALBUMS---");
                foreach (var album in albums)
                {
                    Console.WriteLine("Artist: {0}, Album: {1}", album.Artist, album.Title);
                }
            }
        }
    }
}