using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class AlbumSearchScraperCombiner
    {
        /// <summary>
        /// Default page count is set to 20
        /// </summary>
        /// <param name="numberOfAlbumsInTotal"></param>
        /// <returns></returns>
        public int GetPageCount(int numberOfAlbumsInTotal)
        {
            return (int) Math.Ceiling((double) numberOfAlbumsInTotal/20);
        }

        public IEnumerable<AlbumSearchResult> CombinePages(string[] pages)
        {
            //TODO: refactoring candidate 

            var temp = new List<AlbumSearchResult>();

            foreach (var page in pages)
            {
                var searchScraper = new AlbumSearchScraper(page);

                temp.AddRange(searchScraper.ScrapeAlbums());
            }

            return temp;
        }
    }
}