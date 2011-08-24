using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.IO.Mp4Tagger;
using ZuneSocialTagger.Tests.SampleData;

namespace ZuneSocialTagger.Tests.Unit
{
    [TestFixture]
    public class ZuneXtraParserTests
    {
        [Test]
        public void A_guid_part_should_be_able_to_construct_itself_as_raw_byte_data() { 
            var part = new GuidPart("ZuneMediaID", new Guid("{9a389106-0100-11db-89ca-0019b92a3933}"));
            byte[] rawData = part.Render();
            CollectionAssert.AreEqual(Raw.ZuneMediaIDPartWithLengthAndName, rawData);
        }

        [Test]
        public void A_raw_part_should_be_able_to_construct_itself_as_raw_byte_data()
        {
            var part = new RawPart { Name = "WM/SharedUserRating" };
            //skip the part length, name length and name
            part.Content = Raw.WMSharedUserRatingWithLengthAndName.Skip(27).ToArray();

            byte[] rawData = part.Render();
            CollectionAssert.AreEqual(Raw.WMSharedUserRatingWithLengthAndName, rawData);
        }

        [Test]
        public void Should_be_able_to_construct_objects_from_raw_xtra_part_data()
        {
            var parts = ZuneXtraParser.ParseRawData(Raw.XtraPartWithoutLengthAndName);

            Assert.AreEqual(new Guid("{95cb0200-0600-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneAlbumArtistMediaID").Cast<GuidPart>().First().Guid);

            Assert.AreEqual(new Guid("{8ef21ad1-479b-424e-8277-ae3dab16062d}"), 
                parts.Where(x=> x.Name == "ZuneCollectionID").Cast<GuidPart>().First().Guid);

            Assert.IsNotNull(parts.Where(x => x.Name == "ZuneUserEditedFields").Cast<RawPart>().First());

            Assert.AreEqual(new Guid("{dd650c00-0400-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneAlbumMediaID").Cast<GuidPart>().First().Guid);

            Assert.AreEqual(new Guid("{c6478900-0500-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneMediaID").Cast<GuidPart>().First().Guid);

            Assert.IsNotNull(parts.Where(x => x.Name == "WM/UniqueFileIdentifier").Cast<RawPart>().First());
        }

        [Test]
        public void Should_be_able_to_create_the_raw_data_from_the_parsed_object()
        {
            //parse raw data
            List<IBasePart> parts = ZuneXtraParser.ParseRawData(Raw.XtraPartWithWMSharedUserRating).ToList();
            byte[] constructedRawData = ZuneXtraParser.ConstructRawData(parts);
            CollectionAssert.AreEqual(Raw.XtraPartWithWMSharedUserRating, constructedRawData);
        }

        [Test]
        public void Should_be_able_to_create_the_raw_data_from_the_parsed_object_complex()
        {
            //parse raw data
            List<IBasePart> parts = ZuneXtraParser.ParseRawData(Raw.XtraPartWithoutLengthAndName).ToList();
            byte[] constructedRawData = ZuneXtraParser.ConstructRawData(parts);
            CollectionAssert.AreEqual(Raw.XtraPartWithoutLengthAndName, constructedRawData);
        }
    }
}
