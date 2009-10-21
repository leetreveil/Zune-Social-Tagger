using System;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;


namespace ZuneSocialTagger.IntegrationTests
{
    public class Program
    {
        private static string[] _webpages = new[] { "http://social.zune.net/album/The-Cribs/Ignore-The-Ignorant/37b9f201-0100-11db-89ca-0019b92a3933/details", 
                                                    "http://social.zune.net/album/Kid-Cudi/Man-On-The-Moon--The-End-Of-Day-(Deluxe-Version)-(Parental-Advisory)/eb54f601-0100-11db-89ca-0019b92a3933/details",
                                                    "http://social.zune.net/album/Evil-Activities/Evilution/6f621500-0400-11db-89ca-0019b92a3933/details",
                                                    "http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details",
                                                    "http://social.zune.net/album/Pendulum/Live-at-Brixton-Academy/febc1800-0400-11db-89ca-0019b92a3933/details"};

        static void Main()
        {
            Console.WriteLine("downloading {0} zune album webpages...", _webpages.Length);

            foreach (var url in _webpages)
            {
                string webpageData = PageDownloader.Download(url);

                Console.WriteLine("");
                Console.WriteLine("successfully downloaded {0}", url);
                Console.WriteLine("");

                try
                {
                    var scraper = new AlbumMediaIDScraper(webpageData);

                    foreach (var song in scraper.Scrape())
                    {
                        Console.WriteLine(song);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("could not read song data :(");
                }


            }

            Console.ReadLine();
        }
    }
}