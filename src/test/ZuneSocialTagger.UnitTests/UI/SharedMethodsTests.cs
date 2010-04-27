using NUnit.Framework;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI
{
    public class SharedMethodsTests
    {
        [Test]
        public void Should_return_linked_if_artist_starts_with_the()
        {
            var result = SharedMethods.GetAlbumLinkStatus("Shape Of Punk To Come", "The Refused", "Shape Of Punk To Come", "Refused");

            Assert.That(result, Is.EqualTo(LinkStatus.Linked));
        }

        [Test]
        public void Should_return_linked_if_artist_has_a_accent_in_and_source_does_not()
        {
            var result = SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros", "Takk", "Sigur Rós");

            Assert.That(result,Is.EqualTo(LinkStatus.Linked));
        }

        [Test]
        public void Should_return_linked_if_artist_starts_with_A_and_other_does_not()
        {
            var result = SharedMethods.GetAlbumLinkStatus("Takk", "A Sigur Ros", "Takk", "Sigur Ros");

            Assert.That(result, Is.EqualTo(LinkStatus.Linked));
        }

        [Test]
        public void Should_return_linked_if_artist_ends_with_car_and_other_does_not()
        {
            var result = SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros.", "Takk", "Sigur Ros");

            Assert.That(result, Is.EqualTo(LinkStatus.Linked));
        }

        [Test]
        public void Should_return_linked_if_artist_ends_with_car_and_other_does_not_2()
        {
            var result = SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros?", "Takk", "Sigur Ros");

            Assert.That(result, Is.EqualTo(LinkStatus.Linked));
        }
    }
}