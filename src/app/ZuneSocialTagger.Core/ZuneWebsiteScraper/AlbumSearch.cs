using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class AlbumSearch
    {
        public static IEnumerable<AlbumSearchResult> SearchFor(string artist)
        {
            return GetAlbumFromPages(artist, GetPageCount(artist));
        }

        public static void SearchForAsync(string artist,Action<IEnumerable<AlbumSearchResult>> callback)
        {
            ThreadPool.QueueUserWorkItem(state => GetAlbumFromPagesCallbackWithCompletedEvent(artist, GetPageCount(artist), callback));
        }

        private static IEnumerable<AlbumSearchResult> GetAlbumFromPages(string artist, int pageCount)
        {
            var listOAlbums = new List<AlbumSearchResult>();

            GetAlbumFromPagesCallback(artist,pageCount, listOAlbums.AddRange);

            return listOAlbums;
        }

        private static void GetAlbumFromPagesCallback(string artist,int pageCount, Action<IEnumerable<AlbumSearchResult>> callback)
        {
            for (int i = 0; i < pageCount; i++)
                callback(GetPage(i, artist));
        }

        private static void GetAlbumFromPagesCallbackWithCompletedEvent(string artist, int pageCount, Action<IEnumerable<AlbumSearchResult>> callback)
        {
            GetAlbumFromPagesCallback(artist, pageCount, callback);
            InvokeSearchForAsyncCompleted();
        }

        private static IEnumerable<AlbumSearchResult> GetPage(int pageIndex, string artist)
        {
            string page = AlbumSearchUrlGenerator.CreateUrl(artist, pageIndex + 1);

            var newPageScraper = new AlbumSearchScraper(
                PageDownloader.Download(page));

            return newPageScraper.ScrapeAlbums();
        }

        private static int GetPageCount(string artist)
        {
            string url = AlbumSearchUrlGenerator.CreateUrl(artist);

            string firstAlbumPage = PageDownloader.Download(url);

            var scraper = new AlbumSearchScraper(firstAlbumPage);

            var combiner = new AlbumSearchScraperCombiner();

            return combiner.GetPageCount(scraper.ScrapeAlbumCountAcrossAllPages());
        }

        public static event Action SearchForAsyncCompleted;

        private static void InvokeSearchForAsyncCompleted()
        {
            Action completed = SearchForAsyncCompleted;
            if (completed != null) completed();
        }
    }
}