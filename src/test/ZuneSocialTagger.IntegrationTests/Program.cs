using System;
using System.Collections.Generic;
using System.Windows;
using ID3Tag.HighLevel;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using System.Diagnostics;
using ID3Tag;
using ZuneSocialTagger.Core.ID3Tagger;

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