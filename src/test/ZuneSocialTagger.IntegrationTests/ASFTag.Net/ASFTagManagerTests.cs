using System.Collections.Generic;
using NUnit.Framework;
using ASFTag.Net;
using System.Linq;
using System;
using Attribute=ASFTag.Net.Attribute;

namespace ZuneSocialTagger.IntegrationTests.ASFTag.Net
{
    [TestFixture]
    public class ASFTagManagerTests
    {
        private string _file = "SampleData/asfheadercomplete.wma";

        [Test]
        public void Should_be_able_to_get_tag_container_object_from_a_wma_file()
        {
            TagContainer container = ASFTagManager.ReadTag(_file);

            Assert.That(container, Is.Not.Null);
            Assert.That(container, Is.Not.Empty);
        }

        [Test]
        public void Should_be_able_to_write_an_updated_tag_container_back_to_file_with_a_new_value()
        {
            TagContainer container = ASFTagFactory.CreateASFTagContainer();

            var attrib = new Attribute("ANewId", "Hello", WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            container.Add(attrib);

            ASFTagManager.WriteTag(_file,container);

            TagContainer tag = ASFTagManager.ReadTag(_file);

            var newFrame = tag.Where(x => x.Name == "ANewId").First();

            Assert.That(newFrame.Type,Is.EqualTo(WMT_ATTR_DATATYPE.WMT_TYPE_STRING));
            Assert.That(newFrame.Value,Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_be_able_to_update_an_existing_frame_when_writing_the_container_back_to_file()
        {
            TagContainer newContainer = ASFTagFactory.CreateASFTagContainer();

            //I know that there is a frame inside the file with an id of Title and is a string datatype
            var attrib = new Attribute("Title", "NEW", WMT_ATTR_DATATYPE.WMT_TYPE_STRING);

            newContainer.Add(attrib);

            ASFTagManager.WriteTag(_file, newContainer);

            TagContainer container = ASFTagManager.ReadTag(_file);

            var updatedFrames = container.Where(x => x.Name == "Title");

            if (updatedFrames.Count() > 1)
                Assert.Fail("found the same title frame more than once");

            var updatedFrame = updatedFrames.First();

            Assert.That(updatedFrame.Type, Is.EqualTo(WMT_ATTR_DATATYPE.WMT_TYPE_STRING));
            Assert.That(updatedFrame.Value, Is.EqualTo("NEW"));
        }

        [Test]
        public void Should_be_able_to_write_a_new_guid_to_file()
        {
            TagContainer newContainer = ASFTagFactory.CreateASFTagContainer();

            string aguid = Guid.NewGuid().ToString();

            var attrib = new Attribute("SomeID", aguid, WMT_ATTR_DATATYPE.WMT_TYPE_GUID);

            newContainer.Add(attrib);

            ASFTagManager.WriteTag(_file, newContainer);

            TagContainer tag = ASFTagManager.ReadTag(_file);

            var newFrame = tag.Where(x => x.Name == "SomeID").First();

            Assert.That(newFrame.Type,Is.EqualTo(WMT_ATTR_DATATYPE.WMT_TYPE_GUID));
            Assert.That(newFrame.Value,Is.EqualTo(aguid));
        }

        [Test]
        public void Should_be_able_to_modify_an_existing_guid()
        {
            TagContainer container = ASFTagFactory.CreateASFTagContainer();

            string aguid = Guid.NewGuid().ToString();

            var attrib = new Attribute("ZuneMediaID", aguid, WMT_ATTR_DATATYPE.WMT_TYPE_GUID);

            container.Add(attrib);

            ASFTagManager.WriteTag(_file, container);

            var updatedFrames = ASFTagManager.ReadTag(_file).Where(x => x.Name == "ZuneMediaID");

            if (updatedFrames.Count() > 1)
                Assert.Fail("found the same title frame more than once");

            var result = updatedFrames.First();

            Assert.That(result.Value, Is.EqualTo(aguid));
        }

        [Test]
        public void Should_be_able_to_add_all_the_different_datatypes_as_new_values()
        {
            TagContainer container = ASFTagFactory.CreateASFTagContainer();

            var stringAttrib = new Attribute("One", "some text", WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            var guidAttrib = new Attribute("Two", Guid.NewGuid().ToString(), WMT_ATTR_DATATYPE.WMT_TYPE_GUID);
            var boolAttrib = new Attribute("Three", "1", WMT_ATTR_DATATYPE.WMT_TYPE_BOOL);
            var dwordAttrib = new Attribute("Four", UInt32.MaxValue.ToString(), WMT_ATTR_DATATYPE.WMT_TYPE_DWORD);
            var qwordAttrib = new Attribute("Five", UInt64.MaxValue.ToString(), WMT_ATTR_DATATYPE.WMT_TYPE_QWORD);
            var wordAttrib = new Attribute("Six", UInt16.MaxValue.ToString(), WMT_ATTR_DATATYPE.WMT_TYPE_WORD);

            container.AddRange(new List<Attribute>{stringAttrib,guidAttrib,boolAttrib,dwordAttrib,qwordAttrib,wordAttrib});

            ASFTagManager.WriteTag(_file,container);

            TagContainer updatedContainer = ASFTagManager.ReadTag(_file);

            Attribute newStringAttrib = updatedContainer.Where(x => x.Name == stringAttrib.Name).First();
            Attribute newGuidAttrib = updatedContainer.Where(x => x.Name == guidAttrib.Name).First();
            Attribute newBoolAttrib = updatedContainer.Where(x => x.Name == boolAttrib.Name).First();
            Attribute newdwordAttrib = updatedContainer.Where(x => x.Name == dwordAttrib.Name).First();
            Attribute newqwordAttrib = updatedContainer.Where(x => x.Name == qwordAttrib.Name).First();
            Attribute newwordAttrib = updatedContainer.Where(x => x.Name == wordAttrib.Name).First();

           Assert.That(newStringAttrib.Value,Is.EqualTo(stringAttrib.Value));
           Assert.That(newGuidAttrib.Value, Is.EqualTo(newGuidAttrib.Value));
           Assert.That(newBoolAttrib.Value, Is.EqualTo(newBoolAttrib.Value));
           Assert.That(newdwordAttrib.Value, Is.EqualTo(newdwordAttrib.Value));
           Assert.That(newqwordAttrib.Value, Is.EqualTo(newqwordAttrib.Value));
           Assert.That(newwordAttrib.Value, Is.EqualTo(newwordAttrib.Value));
        }

        [Test]
        public void Should_be_able_to_add_a_string_which_is_split_over_3_different_attributes()
        {
            //there is already 3 author fields in the file, this should update them all
            TagContainer container = ASFTagFactory.CreateASFTagContainer();

            //this should output 3 new attributes
            var contributingArtists = new Attribute("Author", "Andy C;Armin Van Burren;U2",
                                                    WMT_ATTR_DATATYPE.WMT_TYPE_STRING);

            container.Add(contributingArtists);

            ASFTagManager.WriteTag(_file,container);

            TagContainer updatedContainer = ASFTagManager.ReadTag(_file);

            Attribute first = updatedContainer.Where(x => x.Value == "Andy C").First();
            Attribute second = updatedContainer.Where(x => x.Value == "Armin Van Burren").First();
            Attribute third = updatedContainer.Where(x => x.Value == "U2").First();

            Assert.That(first.Name,Is.EqualTo("Author"));
            Assert.That(second.Name, Is.EqualTo("Author"));
            Assert.That(third.Name, Is.EqualTo("Author"));
        }

        [Test]
        public void Should_not_do_anything_if_the_container_is_empty()
        {
            TagContainer container = ASFTagFactory.CreateASFTagContainer();

            ASFTagManager.WriteTag(_file,container);
        }
    }
}

