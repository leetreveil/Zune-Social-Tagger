using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class AlbumSearch
    {
        public static IEnumerable<AlbumSearchResult> SearchFor(string artist)
        {
            var tempList = new List<AlbumSearchResult>();
            string url = AlbumSearchUrlGenerator.CreateUrl(artist);

            string firstAlbumPage = PageDownloader.Download(url);

            var scraper = new AlbumSearchScraper(firstAlbumPage);
  
            var combiner = new AlbumSearchScraperCombiner();

            int pageCount = combiner.GetPageCount(scraper.ScrapeAlbumCountAcrossAllPages());

            //TODO: fix this
            //NOTE: this is hugely inefficient because we are downloading the first page twice!
            for (int i = 0; i < pageCount; i++)
            {
                // + 1 because the url starts at 1 and i starts at 0
                string page = AlbumSearchUrlGenerator.CreateUrl(artist, i +1);
                var newPageScraper =
                    new AlbumSearchScraper(
                        PageDownloader.Download(page));

                tempList.AddRange(newPageScraper.ScrapeAlbums());
            }

            return tempList;
        }
    }
}