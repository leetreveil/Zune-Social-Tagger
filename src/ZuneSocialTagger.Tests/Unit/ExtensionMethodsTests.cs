using System;
using NUnit.Framework;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.Tests.Unit
{
    [TestFixture]
    public class ExtensionMethodTests
    {
        [Test]
        public void Should_be_able_to_convert_a_valid_uuid_to_a_guid()
        {
            var guid = ExtensionMethods
                .ExtractGuidFromUrnUuid("urn:uuid:c14c4e00-0300-11db-89ca-0019b92a3933");

            Assert.AreEqual(new Guid("c14c4e00-0300-11db-89ca-0019b92a3933"), guid);
        }

        [Test]
        public void Should_return_an_empty_guid_when_converting_an_invalid_uuid()
        {
            var guid = ExtensionMethods
                .ExtractGuidFromUrnUuid("c14c4e00-0300-11db-89ca-0019b92a3933");

            Assert.AreEqual(Guid.Empty, guid);
        }

        [Test]
        public void Should_return_an_empty_guid_when_converting_a_random_uuid_string()
        {
            var guid = ExtensionMethods
                .ExtractGuidFromUrnUuid("SOME TEXT:SOMETEXT");

            Assert.AreEqual(Guid.Empty, guid);
        }
    }
}