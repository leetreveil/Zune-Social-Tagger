using NUnit.Framework;
using ZuneSocialTagger.Core;
using System;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Collections.Generic;

namespace ZuneSocialTagger.UnitTests.Core
{
    [TestFixture]
    public class ExtensionMethodsTests
    {
        #region IEnumerable<Track> Tests

        private readonly Track _validGuid = new Track { MediaID = Guid.NewGuid(), Title = "N/A",ArtistMediaID = Guid.NewGuid()};
        private readonly Track _guidWithNullGuid = new Track { Title = "Hello" };
        private readonly Track _guidWithBlankGuid = new Track { MediaID = Guid.Empty };
        private readonly Track _guidWithBlankTitleAndValidGuid = new Track{MediaID = Guid.NewGuid(),Title = String.Empty};
        private readonly Track _guidWithNullTitleAndValidGuid = new Track { MediaID = Guid.NewGuid() };

        [Test]
        public void Should_return_true_if_all_guids_have_a_title_and_a_non_empty_or_null_guid()
        {
            var guids = new List<Track> { _validGuid };

            Assert.That(guids.AreAllValid(), Is.True);
        }

        [Test]
        public void Should_return_false_if_any_guids_are_null()
        {
            Assert.That(new List<Track> { _guidWithNullGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_any_guids_are_blank_0_s()
        {
            Assert.That(new List<Track> { _guidWithBlankGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_true_if_all_guids_are_valid()
        {
            Assert.That(new List<Track> { _validGuid, _validGuid, _validGuid }.AreAllValid(), Is.True);
        }

        [Test]
        public void Should_return_false_if_all_guids_are_valid_apart_from_one()
        {
            Assert.That(new List<Track> { _validGuid, _validGuid, _validGuid, _guidWithNullGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_list_is_empty()
        {
            Assert.That(new List<Track>().AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_title_is_blank()
        {
            Assert.That(new List<Track>{_guidWithBlankTitleAndValidGuid}.AreAllValid(),Is.False);
        }

        [Test]
        public void Should_return_false_if_title_is_null()
        {
            Assert.That(new List<Track> { _guidWithNullTitleAndValidGuid }.AreAllValid(), Is.False);
        }

        #endregion

        #region UrlTests

        [TestCase("http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=true&width=240&height=240",Result = "http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=false&width=480&height=480")]
        [TestCase("http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=false&width=240&height=240", Result = "http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=false&width=480&height=480")]
        [TestCase("http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=true&width=75&height=75", Result = "http://image.catalog.zune.net/v3.0/image/7510d300-0300-11db-89ca-0019b92a3933?resize=false&width=480&height=480")]
        public string Should_be_able_to_convert_a_standard_zune_artwork_url_to_its_non_resised_version(string input)
        {
            return input.ConvertToNonResizedImageUrl();
        }


        #endregion

    }
}