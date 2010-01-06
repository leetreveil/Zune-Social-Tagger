using System;

namespace ZuneSocialTagger.IntegrationTests
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("1 for album webpage scraper, 2 for artist search");
            string line = Console.ReadLine();

            if (line == "1")
            {
                AlbumWebpageScraperTestHarness.Main();
            }
            if (line == "2")
            {
                AlbumSearchTestHarness.Main();
            }
        }
    }
}