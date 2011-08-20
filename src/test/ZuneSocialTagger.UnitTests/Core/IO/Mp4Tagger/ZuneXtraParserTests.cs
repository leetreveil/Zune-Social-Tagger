using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.IO.Mp4Tagger;

namespace leetreveil.UnitTests.Core.IO.Mp4Tagger
{
    [TestFixture]
    public class ZuneXtraParserTests
    {
        [Test]
        public void Should_be_able_to_construct_objects_from_raw_xtra_part_data()
        {
            var parts = ZuneXtraParser.ParseRawData(SampleData.XtraPartWithLengthAndName);

            Assert.AreEqual(new Guid("{95cb0200-0600-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneAlbumArtistMediaID").Cast<GuidPart>().First().MediaId);

            Assert.AreEqual(new Guid("{8ef21ad1-479b-424e-8277-ae3dab16062d}"), 
                parts.Where(x=> x.Name == "ZuneCollectionID").Cast<GuidPart>().First().MediaId);

            Assert.AreEqual(new byte[] { 0x1c, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 
                parts.Where(x=> x.Name == "ZuneUserEditedFields").Cast<RawPart>().First().Content);

            Assert.AreEqual(new Guid("{dd650c00-0400-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneAlbumMediaID").Cast<GuidPart>().First().MediaId);

            Assert.AreEqual(new Guid("{c6478900-0500-11db-89ca-0019b92a3933}"), 
                parts.Where(x=> x.Name == "ZuneMediaID").Cast<GuidPart>().First().MediaId);

            Assert.AreEqual(114, 
                parts.Where(x => x.Name == "WM/UniqueFileIdentifier").Cast<RawPart>().First().Content.Length);
        }

        //[Test]
        //public void Should_be_able_to_create_the_raw_data_from_the_parsed_objects()
        //{
        //    //parse raw data
        //    List<IBasePart> parts = ZuneXtraParser.ParseRawData(SampleData.XtraPartWithLengthAndName).ToList();
        //    byte[] constructedRawData = ZuneXtraParser.ConstructRawData(parts);

        //    CollectionAssert.AreEqual(SampleData.RawDataWithLengthAndNamePrefix, constructedRawData);
        //}
    }
}
