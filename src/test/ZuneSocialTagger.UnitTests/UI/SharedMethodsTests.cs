using NUnit.Framework;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI
{
    public class SharedMethodsTests
    {
        #region GetAlbumListStatus Tests

        [Test]
        public void ShouldMatchIfArtistNameStartsWithThe()
        {
            bool result = SharedMethods.DoesStringMatchWithoutTheAtStart("The Shape Of Punk To Come", "Shape Of Punk To Come");

            Assert.That(result,Is.True);
        }

        [Test]
        public void ShouldNotMatchIfArtistNameDoesNotStartWithThe()
        {
            bool result = SharedMethods.DoesStringMatchWithoutTheAtStart("AFI", "U2");

            Assert.That(result, Is.True);
        }

        #endregion
    }
}