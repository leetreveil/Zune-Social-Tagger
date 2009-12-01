using NUnit.Framework;
using ZuneSocialTagger.Core;
using System;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Collections.Generic;

namespace ZuneSocialTagger.UnitTests.Core.ZuneWebsite
{
    [TestFixture]
    public class TrackTests
    {
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

    }
}