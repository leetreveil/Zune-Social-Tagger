using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using System.Linq;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenProvidedWithAnAlbumSearchWebpageThatHasTwoPagesAvailable
    {
        private const string PathToFile = "SampleData/albumsearchresult.xml";
        private readonly string _fileData;

        public WhenProvidedWithAnAlbumSearchWebpageThatHasTwoPagesAvailable()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Then_it_should_be_able_to_calculate_the_number_of_pages_required_to_download_all_the_albums()
        {
            var combiner = new ZuneArtistSearchScraperCombiner();

            int result =  combiner.GetPageCount(38);

            Assert.That(result,Is.EqualTo(2));
        }

        [Test]
        public void Then_it_should_be_able_to_combine_the_results_of_both_pages()
        {
            var combiner = new ZuneArtistSearchScraperCombiner();

            string[] pageData = new[]{_fileData,_fileData};

            IEnumerable<AlbumSearchResult> result = combiner.CombinePages(pageData);

            //_fileData has 18 albums in it so therefore 2 of the same is 36
            Assert.That(result.Count(), Is.EqualTo(36));
        }

    }
}