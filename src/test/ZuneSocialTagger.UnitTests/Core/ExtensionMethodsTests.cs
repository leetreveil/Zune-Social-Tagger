using NUnit.Framework;
using ZuneSocialTagger.Core;
using System;

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
    }
}