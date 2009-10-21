using System;
using NUnit.Framework;
using System;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.UnitTests.Core
{
    [TestFixture]
    public class ZuneIDConverterTests
    {
        [Test]
        public void Should_be_able_to_convert_a_ZuneAlbumArtistMediaID_to_a_byte_array()
        {
            string zuneAlbumArtistMediaID = "28cc0700-0600-11db-89ca-0019b92a3933";

            byte[] data = ZuneIDConverter.Convert(zuneAlbumArtistMediaID);

            //2 byte padding at start
            string expectedOutput = "000007CC280006DB1189CA0019B92A3933";

            Assert.That(ZuneIDConverter.ByteArrayToString(data).ToUpper(),Is.StringMatching(expectedOutput));
        }

        [Test]
        public void Should_be_able_to_convert_a_ZuneAlbumMediaID_to_a_byte_array()
        {
            string zuneAlbumArtistMediaID = "677cf801-0100-11db-89ca-0019b92a3933";

            byte[] data = ZuneIDConverter.Convert(zuneAlbumArtistMediaID);

            //2 byte padding at start
            string expectedOutput = "0001f87c670001db1189ca0019b92a3933";

            Assert.That(ZuneIDConverter.ByteArrayToString(data), Is.StringMatching(expectedOutput));
        }

        [Test]
        public void Should_be_able_to_convert_a_ZuneMediaID_to_a_byte_array()
        {
            string zuneAlbumArtistMediaID = "697cf801-0100-11db-89ca-0019b92a3933";

            byte[] data = ZuneIDConverter.Convert(zuneAlbumArtistMediaID);

            //2 byte padding at start
            string expectedOutput = "0001f87c690001db1189ca0019b92a3933";

            Assert.That(ZuneIDConverter.ByteArrayToString(data), Is.StringMatching(expectedOutput));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Should_throw_exception_if_the_id_is_shorter_than_36()
        {
            string zuneAlbumArtistMediaID = "28cc0700-0600-11db-89ca-0019b92a393";

            ZuneIDConverter.Convert(zuneAlbumArtistMediaID);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Should_throw_exception_if_the_id_is_longer_than_36()
        {
            string zuneAlbumArtistMediaID = "28cc0700-0600-11db-89ca-0019b92a39333";

            ZuneIDConverter.Convert(zuneAlbumArtistMediaID);
        }
    }
}