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
        #region Guid Tests

        [TestCase("37b9f201-0100-11db-89ca-0019b92a3933#album#Ignore The Ignorant",Result = "37b9f201-0100-11db-89ca-0019b92a3933")]
        [TestCase("FanClub00710a00-0600-11db-89ca-0019b92a3933", Result = "00710a00-0600-11db-89ca-0019b92a3933")]
        [TestCase("-00710a00-0600-11db-89ca-0019b92a3933", Result = "00710a00-0600-11db-89ca-0019b92a3933")]
        [TestCase("00710a00-0600-11db-89ca-0019b92a3933", Result = "00710a00-0600-11db-89ca-0019b92a3933")]
        public string When_extracting_a_valid_guid_from_a_string_with_other_text_in_it_it_should_return_the_guid(string input)
        {
            //note we are returning a string because we cant use new Guid() inside the TestCase result paramater
            return input.ExtractGuid().ToString();
        }


        [Test]
        [ExpectedException(typeof(FormatException))]
        public void When_no_guid_is_present_in_a_string_it_should_throw_an_exception()
        {
            string noGuidPresent = "hello";

            noGuidPresent.ExtractGuid();
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void When_an_invalid_guid_is_present_in_a_string_it_should_throw_a_exception()
        {
            //3 removed from end
            string invalidGuid = "FanClub00710a00-0600-11db-89ca-0019b92a393";

            invalidGuid.ExtractGuid();
        }


        #endregion

        #region IEnumerable<SongGuid> Tests

        private readonly SongGuid _validGuid = new SongGuid { Guid = Guid.NewGuid(), Title = "N/A" };
        private readonly SongGuid _guidWithNullGuid = new SongGuid { Title = "Hello" };
        private readonly SongGuid _guidWithBlankGuid = new SongGuid { Guid = Guid.Empty };
        private readonly SongGuid _guidWithBlankTitleAndValidGuid = new SongGuid{Guid = Guid.NewGuid(),Title = String.Empty};
        private readonly SongGuid _guidWithNullTitleAndValidGuid = new SongGuid { Guid = Guid.NewGuid() };

        [Test]
        public void Should_return_true_if_all_guids_have_a_title_and_a_non_empty_or_null_guid()
        {
            var guids = new List<SongGuid> { _validGuid };

            Assert.That(guids.AreAllValid(), Is.True);
        }

        [Test]
        public void Should_return_false_if_any_guids_are_null()
        {
            Assert.That(new List<SongGuid> { _guidWithNullGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_any_guids_are_blank_0_s()
        {
            Assert.That(new List<SongGuid> { _guidWithBlankGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_true_if_all_guids_are_valid()
        {
            Assert.That(new List<SongGuid> { _validGuid, _validGuid, _validGuid }.AreAllValid(), Is.True);
        }

        [Test]
        public void Should_return_false_if_all_guids_are_valid_apart_from_one()
        {
            Assert.That(new List<SongGuid> { _validGuid, _validGuid, _validGuid, _guidWithNullGuid }.AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_list_is_empty()
        {
            Assert.That(new List<SongGuid>().AreAllValid(), Is.False);
        }

        [Test]
        public void Should_return_false_if_title_is_blank()
        {
            Assert.That(new List<SongGuid>{_guidWithBlankTitleAndValidGuid}.AreAllValid(),Is.False);
        }

        [Test]
        public void Should_return_false_if_title_is_null()
        {
            Assert.That(new List<SongGuid> { _guidWithNullTitleAndValidGuid }.AreAllValid(), Is.False);
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