using System;
using System.Text;
using NUnit.Framework;

namespace ZuneSocialTagger.UnitTests.Core
{
    //TODO: Remove this class, only here to verify GUID's work how i expected them to
    [TestFixture]
    public class GuidGeneratorTests
    {
        [Test]
        public void Should_be_able_to_generate_the_correct_guid_ready_to_be_written_to_file()
        {
            Guid guid = new Guid("697cf801-0100-11db-89ca-0019b92a3933");

            byte[] bytes = guid.ToByteArray();

            string s = ByteArrayToString(bytes);

            Assert.That(s, Is.EqualTo("01f87c690001db1189ca0019b92a3933"));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}