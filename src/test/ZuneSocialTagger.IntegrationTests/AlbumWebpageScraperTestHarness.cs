using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core;
using System.Windows.Media.Imaging;
using System.Net;

namespace ZuneSocialTagger.IntegrationTests
{
    public class AlbumWebpageScraperTestHarness
    {
        #region Urls

        private static string[] _webpages = new[]
                                                {
                                                    "http://social.zune.net/album/Them-Crooked-Vultures/Them-Crooked-Vultures/95dc1002-0100-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/The-Cribs/Ignore-The-Ignorant/37b9f201-0100-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Kid-Cudi/Man-On-The-Moon--The-End-Of-Day-(Deluxe-Version)-(Parental-Advisory)/eb54f601-0100-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Evil-Activities/Evilution/6f621500-0400-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Pendulum/Live-at-Brixton-Academy/febc1800-0400-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Editors/In-This-Light-And-On-This-Evening/4f66ff01-0100-11db-89ca-0019b92a3933/details"
                                                    ,
                                                    "http://social.zune.net/album/Nirvana/Live-At-Reading-(1992)/355c0802-0100-11db-89ca-0019b92a3933/details"
                                                };

        #endregion

        [STAThread]
        public static void Main()
        {
            Console.WriteLine("downloading {0} zune album webpages...", _webpages.Length);

            foreach (var url in _webpages)
            {
                string webpageData = PageDownloader.Download(url);

                Console.WriteLine("");
                Console.WriteLine("successfully downloaded {0}", url);
                Console.WriteLine("");


                var scraper = new AlbumWebpageScraper(webpageData);
                AlbumWebpageScrapeResult result = scraper.Scrape();

                Console.WriteLine("ZuneAlbumArtistID: {0}", result.AlbumArtistID);
                Console.WriteLine("ZuneAlbumMediaID: {0}", result.AlbumMediaID);
                Console.WriteLine("Album Artist: {0}", result.AlbumArtist);
                Console.WriteLine("Title: {0}", result.AlbumTitle);
                Console.WriteLine("Release Year: {0}", result.AlbumReleaseYear);
                Console.WriteLine("Artwork url: {0}", result.AlbumArtworkUrl);

                Console.WriteLine("Does this page have the minimum required info: {0}", result.IsValid());
                Console.WriteLine("");


                //foreach (var song in songAndTitles)
                //{
                //    Console.WriteLine("{0} == {1}", song.Title, song.Guid);
                //}
            }

            Console.WriteLine("finished...");
            Console.ReadLine();
        }
    }
}