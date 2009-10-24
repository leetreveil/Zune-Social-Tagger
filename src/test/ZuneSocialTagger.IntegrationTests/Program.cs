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
        private static string[] _webpages = new[]
                                                {
                                                    //"http://social.zune.net/album/The-Cribs/Ignore-The-Ignorant/37b9f201-0100-11db-89ca-0019b92a3933/details"
                                                    //,
                                                    //"http://social.zune.net/album/Kid-Cudi/Man-On-The-Moon--The-End-Of-Day-(Deluxe-Version)-(Parental-Advisory)/eb54f601-0100-11db-89ca-0019b92a3933/details"
                                                    //,
                                                    //"http://social.zune.net/album/Evil-Activities/Evilution/6f621500-0400-11db-89ca-0019b92a3933/details"
                                                    //,
                                                    //"http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details"
                                                    //,
                                                    //"http://social.zune.net/album/Pendulum/Live-at-Brixton-Academy/febc1800-0400-11db-89ca-0019b92a3933/details"
                                                    //,
                                                    "http://social.zune.net/album/Editors/In-This-Light-And-On-This-Evening/4f66ff01-0100-11db-89ca-0019b92a3933/details"
                                                };

        private static void Main()
        {
            //TagContainer container = Id3TagManager.ReadV2Tag(
            //  "SampleData/Editors - In This Light And On This Evening/onemediaidthatisincorrect.mp3");

            //ZuneTagContainer zuneContainer = new ZuneTagContainer(container);

            //IEnumerable<MediaIdGuid> enumerable = zuneContainer.ReadMediaIds();
            //zuneContainer.WriteMediaIdGuidsToContainer(new List<MediaIdGuid>(){new MediaIdGuid(){Guid = Guid.NewGuid(),MediaId = "ZuneAlbumArtistMediaId"}});

            //Id3TagManager.WriteV2Tag("SampleData/Editors - In This Light And On This Evening/onemediaidthatisincorrect.mp3",container);


            Console.WriteLine("downloading {0} zune album webpages...", _webpages.Length);

            foreach (var url in _webpages)
            {
                string webpageData = PageDownloader.Download(url);

                Console.WriteLine("");
                Console.WriteLine("successfully downloaded {0}", url);
                Console.WriteLine("");


                var scraper = new ZuneAlbumWebpageScraper(webpageData);

                Console.WriteLine("ZuneAlbumArtistID: {0}", scraper.ScrapeAlbumArtistID());
                Console.WriteLine("ZuneAlbumMediaID: {0}", scraper.ScrapeAlbumMediaID());
                Console.WriteLine("Album Artist: {0}",scraper.ScrapeAlbumArtist());
                Console.WriteLine("Title: {0}",scraper.ScrapeAlbumTitle());
                Console.WriteLine("Release Year: {0}",scraper.ScrapeAlbumReleaseYear());
                Console.WriteLine("Artwork url: {0}",scraper.ScrapeAlbumArtworkUrl());
                Console.WriteLine("");

                foreach (var song in scraper.GetSongTitleAndIDs())
                {
                    Console.WriteLine("{0} == {1}", song.Title,song.Guid);

                    //For The Editors
                    TagContainer container = Id3TagManager.ReadV2Tag(
                        "SampleData/Editors - In This Light And On This Evening/onemediaidthatisincorrect.mp3");

                    ZuneTagContainer zuneContainer = new ZuneTagContainer(container);

                    MediaIdGuid albumArtist = new MediaIdGuid { Guid = scraper.ScrapeAlbumArtistID(), MediaId = "ZuneAlbumArtistMediaID" };
                    MediaIdGuid album = new MediaIdGuid { Guid = scraper.ScrapeAlbumMediaID(), MediaId = "ZuneAlbumMediaID" };
                    MediaIdGuid track = new MediaIdGuid { Guid = song.Guid, MediaId = "ZuneMediaID" };

                    zuneContainer.WriteMediaIdGuidsToContainer(new List<MediaIdGuid>() {albumArtist, album, track});

                    Id3TagManager.WriteV2Tag("SampleData/Editors - In This Light And On This Evening/onemediaidthatisincorrect.mp3",container);
                }
            }

            Console.WriteLine("finished...");
            Console.ReadLine();
        }
    }
}